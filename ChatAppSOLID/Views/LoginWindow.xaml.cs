using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChatAppSOLID.Services.Commands;
using ChatAppSOLID.Services.Interfaces;
using ChatAppSOLID.Services.NewFolder;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;


namespace ChatAppSOLID
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public bool IsPasswordValid = false;
        public bool IsUsernameValid = false;
        private readonly ChatClient _chatClient = new ChatClient();
        private readonly RecivedMessageHandler _recivedMessageHandler = new RecivedMessageHandler();
        private readonly IPAddress _ipAddress = IPAddress.Parse("192.168.150.113");
        private readonly int _port = 8080;
        public LoginWindow()
        {
            InitializeComponent();
            Task.Run(async () =>
            {
                try
                {
                    await _chatClient.ConnectAsync(_ipAddress, _port);
                    await Task.Run(() => _recivedMessageHandler.RecivedCommandHandlerAsync(_chatClient.ClientSocket));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to connect to server: {ex.Message}");
                }
            }); _recivedMessageHandler.LoginSuccess += LoginSuccess;
            _recivedMessageHandler.LoginFailure += LoginFailure;
        }
       
        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UsernameBox.Text.Length < 8)
            {
                UsernameBox.Background = Brushes.Red;
                RequirementBox.Visibility = Visibility.Visible;
                requierments.Visibility = Visibility.Visible;
                LoginButton.IsEnabled = false;
            }
            else
            {
                PasswordBox.Background = Brushes.White;
                IsUsernameValid = true;

                if (IsUsernameValid && IsPasswordValid)
                {
                    RequirementBox.Visibility = Visibility.Hidden;
                    requierments.Visibility = Visibility.Hidden;
                    LoginButton.IsEnabled = true;
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) //triggered whenever the password in the PasswordBox changes
        {
            if (PasswordBox.Password.Length > 0)
            {
                passwordPlaceHolder.Visibility = Visibility.Hidden;
            }
            else
            {
                passwordPlaceHolder.Visibility = Visibility.Visible;
            }
            if (PasswordBox.Password.Length < 8)
            {
                PasswordBox.Background = Brushes.Red;
                RequirementBox.Visibility = Visibility.Visible;
                requierments.Visibility = Visibility.Visible;
                LoginButton.IsEnabled = false;
            }
            else
            {
                PasswordBox.Background = Brushes.White;
                IsPasswordValid = true;

                if(IsUsernameValid && IsPasswordValid)
                {
                    RequirementBox.Visibility = Visibility.Hidden;
                    requierments.Visibility = Visibility.Hidden;
                    LoginButton.IsEnabled = true;
                }
               
            }
        }

        private async void  LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (!_chatClient.IsConnected)
            {
                var errorWindow = new ConnectionError
                {
                    Owner = this // Make the popup modal and center it over LoginWindow
                };
                errorWindow.ShowDialog(); // Use ShowDialog for modal behavior
            }

            else
            {
                // Generate a temporary SenderId (e.g., Guid.Empty for unauthenticated users)
                Guid senderId = Guid.Empty; // This should be replaced with a real user ID after login or registration

            // Create and execute the LoginCommand using the socket from IChatClient
            var loginCommand = new LoginCommand(username, password, senderId);
            await loginCommand.ExecuteAsync(_chatClient.ClientSocket);
            }

        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Hide();  // Hide login window
        }

        public void LoginSuccess(object sender, string username)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }
        public void LoginFailure(object sender, string error)
        {
            var connectionError = new ConnectionError
            {
                Owner = this
            };
        }
    }
}
