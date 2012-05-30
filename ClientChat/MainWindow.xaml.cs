using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using ChatLib;

namespace ClientChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IClientService
    {
        ManualResetEvent DisconnectWait = new ManualResetEvent(false);
        Client Connection;
        string Address;
        IPAddress IPAddress;
        int Port;
        bool IsLoggingIn = true;
        string Username;
        List<string> UserList = new List<string>();
        BitmapImage MailIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/mail.png"));
        bool IsNotificationShown = false;
        bool _UnreadMessages = false;
        bool UnreadMessages
        {
            get { return _UnreadMessages; }
            set
            {
                _UnreadMessages = value;
                if (_UnreadMessages)
                {
                    this.TaskbarItemInfo.Overlay = MailIcon;
                    Debug.WriteLine("Taskbar overlay set");
                    //this.TaskbarItemInfo.Overlay = (DrawingImage)this.FindResource("Resources/mail.png");
                }
                else
                {
                    this.TaskbarItemInfo.Overlay = null;
                    Debug.WriteLine("Taskbar overlay reset");
                }

            }
        }

        string StatusMessage
        {
            get
            {
                if (!Status.Dispatcher.CheckAccess())
                {
                    string text = "";
                    Status.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        delegate()
                        {
                            text = Status.Text;
                        }
                    ));
                    return text;
                }
                else
                {
                    return Status.Text;
                }
            }
            set
            {
                if (!Status.Dispatcher.CheckAccess())
                {
                    Status.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        delegate()
                        {
                            Status.Text = value;
                        }
                    ));
                }
                else
                {
                    Status.Text = value;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Connection = new Client(this);
            TaskbarItemInfo = new TaskbarItemInfo();
        }

        public void LogMessage(string text)
        {
            if (!Log.Dispatcher.CheckAccess())
            {
                Log.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                    delegate()
                    {
                        LogMessageBase(text, new SolidColorBrush(Colors.Black));
                    }
                ));
            }
            else
            {
                LogMessageBase(text, new SolidColorBrush(Colors.Black));
            }
        }

        private void LogMessageBase(string text, Brush color)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0);
            // Add it to the paragraph and then to the log
            Run run = new Run(text);
            run.Foreground = color;
            paragraph.Inlines.Add(run);
            Log.Blocks.Add(paragraph);
            // Scroll to the bottom
            LogContainer.ScrollToEnd();
        }

        private void UpdateTitle()
        {
            if (!Log.Dispatcher.CheckAccess())
            {
                Log.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                    delegate()
                    {
                        this.Title = "SimpleChat - " + Username;
                    }
                ));
            }
            else
            {
                this.Title = "SimpleChat - " + Username;
            }
        }

        private void Quit()
        {
            Application.Current.Shutdown();
        }

        public void OnConnect(ConnectionState state)
        {
            // Change title
            UpdateTitle();
            IsLoggingIn = false;
            IPEndPoint endPoint = state.Connection.RemoteEndPoint as IPEndPoint;
            string message = "Connected to server";
            StatusMessage = message;
            // Send the clients name so that the server knows that user is there
            byte[] data = TextEncoder.Encode("JOIN " + Username);
            Connection.Send(data);
        }

        public void OnReceive(ConnectionState state)
        {
            string message = TextEncoder.Decode(state.Buffer, state.Length);
            if (message.StartsWith("LIST ")) // user list (TODO)
            {
                string name = message.Replace("LIST ", "");
                UserList.Add(name);
            }
            else
            {
                // Add time stamp on it
                DateTime now = DateTime.Now;
                string date = now.ToString("[MMM dd hh:mm tt]", CultureInfo.InvariantCulture);
                LogMessage(date + " " + message);

                Debug.WriteLine("Message recieved");
                // Check if notification is needed

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                    delegate()
                    {
                        Debug.WriteLine("Is currently active window: " + this.IsActive);
                        if (!this.IsActive)
                        {
                            Debug.WriteLine("Notification shown");
                            NotificationWindow notificaiton = new NotificationWindow(message, this);
                            notificaiton.Show();
                            IsNotificationShown = true;
                            UnreadMessages = true;
                            Flash();
                            Debug.WriteLine("Notifications enabled");
                        }
                    }
                ));
            }

            Debug.WriteLine("<-" + message);
        }

        private void Flash()
        {
            //The Flash-methods now take a handle.
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            FlashWindow.Flash(hwnd, 20);
        }

        public void OnDisconnect(ConnectionState state)
        {
            StatusMessage = "Disconnected from server";
            DisconnectWait.Set();
        }

        public void OnError(ConnectionState state)
        {
            string message = string.Format("Error: {0}", state.Error.Message);
            StatusMessage = message;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoggingIn = true;
            // Login dialog
            LoginDialog dialog = new LoginDialog();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (dialog.ShowDialog() == true)
            {
                Address = dialog.ServerAddress;
                IPAddress = dialog.IPAddress;
                Port = dialog.Port;
                Username = dialog.FirstName + " " + dialog.LastName;
                Thread start = new Thread(new ThreadStart(Connect));
                start.Start();
                string message = string.Format("Connecting to server on {0}:{1}", Address, Port);
                StatusMessage = message;
            }
            else Quit();
        }

        private void Connect()
        {
            Connection.Start(IPAddress, Port);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // Only run is the text is not empty
            if (Message.Text == "") return;
            // Append name to message also
            string message = Username + ": " + Message.Text;
            byte[] data = TextEncoder.Encode(message);
            Connection.Send(data);
            // Clear the message box
            Message.Text = "";

            Debug.WriteLine("->" + message);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (IsLoggingIn)
                    e.Cancel = false;
                else
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to disconnect from the server and quit?", "SimpleChat", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            Disconnect();
                            e.Cancel = false;
                            break;
                        default:
                            e.Cancel = true;
                            break;
                    }
                }
            }
            catch
            {
                e.Cancel = false;
            }
        }

        private void Disconnect()
        {
            if (!Connection.IsConnected) return;

            Connection.Disconnect();
            //DisconnectWait.WaitOne();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Debug.WriteLine("Active but nothing shown");
            if (IsNotificationShown)
            {
                UnreadMessages = false;
                IsNotificationShown = false;
                Debug.WriteLine("Notifications disabled");
            }
        }
    }
}
