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

        public string ServerAddress
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
            if (ServerAddress == string.Empty || FirstName == string.Empty || LastName == string.Empty)
            {
                MessageBox.Show("A field is empty.\nMake sure that you completed them all", "Login",
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                // Remove any trialing spaces on user's name
                FirstNameTextBox.Text = FirstNameTextBox.Text.Trim(' ');
                LastNameTextBox.Text = LastNameTextBox.Text.Trim(' ');
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
            Settings.Default.Address = ServerAddress;
            Settings.Default.FirstName = FirstName;
            Settings.Default.LastName = LastName;
            Settings.Default.Save();
        }
    }
}
