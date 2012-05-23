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
using System.Windows.Shapes;
using System.Net;
using ClientChat.Properties;
using ChatLib;

namespace ClientChat
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public IPAddress IPAddress { get; private set; }

        public int Port { get; private set; }

        public string ServerAddress { get; private set; }

        public string ServerAddressRaw
        {
            get { return AddressTextBox.Text; }
        }

        public string FirstName
        {
            get { return FirstNameTextBox.Text; }
        }

        public string LastName
        {
            get { return LastNameTextBox.Text; }
        }

        public LoginDialog()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
            LoadSettings();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Make sure that nothing is empty
            if (string.IsNullOrWhiteSpace(ServerAddressRaw) || string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("A field is empty.\nMake sure that you completed them all", "Login",
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                bool valid = true;
                // Remove any trialing spaces on user's name
                FirstNameTextBox.Text = FirstNameTextBox.Text.Trim(' ');
                LastNameTextBox.Text = LastNameTextBox.Text.Trim(' ');

                // Split the IP with the port number
                string[] address = ServerAddressRaw.Split(":".ToCharArray(), 2);
                ServerAddress = address[0];

                // Port
                if (address.Length == 1) // address only
                {
                    Port = Globals.Port;
                }
                else // with port
                {
                    int port;
                    if (int.TryParse(address[1], out port))
                    {
                        Port = port;
                    }
                    else
                    {
                        valid = false;
                    }
                }

                // IP address
                if (valid)
                {
                    // Validate the address
                    IPAddress ip;
                    if (Address.TryParseAddress(ServerAddress, out ip))
                    {
                        IPAddress = ip;
                        SaveSettings();
                        this.DialogResult = true;
                    }
                    else
                    {
                        valid = false;
                    }
                }

                // Return error message if invalid
                if (!valid)
                {
                    MessageBox.Show("The server address is invalid.\nMake sure that you entered it correctly and you are connected to the internet", "Login",
                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
        }

        private bool Validate(string text)
        {
            if (text == string.Empty) return false; // Empty
            if (text.Contains(':')) return false; // Colors
            else return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void LoadSettings()
        {
            AddressTextBox.Text = Settings.Default.Address;
            FirstNameTextBox.Text = Settings.Default.FirstName;
            LastNameTextBox.Text = Settings.Default.LastName;
        }

        private void SaveSettings()
        {
            Settings.Default.Address = ServerAddressRaw;
            Settings.Default.FirstName = FirstName;
            Settings.Default.LastName = LastName;
            Settings.Default.Save();
        }
    }
}
