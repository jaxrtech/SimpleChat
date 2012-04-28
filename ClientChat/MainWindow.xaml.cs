using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatLib;
using System.Net;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

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

        public MainWindow()
        {
            Connection = new Client(this);

            InitializeComponent();
        }

        public void LogMessage(string text)
        {
            if (!Log.Dispatcher.CheckAccess())
            {
                Log.Dispatcher.Invoke(
                  DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        LogMessageBase(text);
                    }
                ));
            }
            else
            {
                LogMessageBase(text);
            }

        }

        private void LogMessageBase(string text)
        {
            // Only add a line return if the log is not empty
            string newLine = (Log.Text == "") ? "" : "\n";
            Log.AppendText(newLine + text);
            // SCroll to the bottom
            Log.CaretIndex = Log.Text.Length;
            Log.ScrollToEnd();
        }

        private void UpdateTitle()
        {
            if (!Log.Dispatcher.CheckAccess())
            {
                Log.Dispatcher.Invoke(
                  DispatcherPriority.Normal,
                  new Action(
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
            LogMessage(message);
            // Send the clients name so that the server knows that user is there
            byte[] data = TextEncoder.Encode("JOIN " + Username);
            Connection.Send(data);
        }

        public void OnReceive(ConnectionState state)
        {
            string message = TextEncoder.Decode(state.Buffer, state.Length);
            //if (message.StartsWith("LIST ")) // user list (TODO)
            //{
            //    message.Replace("LIST ", "");
            //    string[] names = message.Split(',');
            //    message = "Current users: ";
            //    int i = 0;
            //    int max = names.Length;
            //    foreach (var name in names)
            //    {
            //        if (i == 0 || i == max) // first or last
            //        {
            //            message += name;
            //        }
            //        else if (i > 0 && i < max - 1) // in the middle
            //        {
            //            message += ", " + name;
            //        }
            //        i++;
            //    }
            //}
            LogMessage(message);

            Debug.WriteLine("<-" + message);
        }

        public void OnDisconnect(ConnectionState state)
        {
            LogMessage("Disconnected from server");
            DisconnectWait.Set();
        }

        public void OnError(ConnectionState state)
        {
            string message = string.Format("Error: {0}", state.Error.Message);
            LogMessage(message);
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
                Port = Globals.Port;
                Username = dialog.FirstName + " " + dialog.LastName;
                Thread start = new Thread(new ThreadStart(Connect));
                start.Start();
                string message = string.Format("Connecting to server on {0}:{1}", Address, Port);
                LogMessage(message);
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
    }
}
