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
using ChatAppSOLID.ViewModels;


namespace ChatAppSOLID
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public bool IsPasswordValid = false;
        public bool IsUsernameValid = false;
        private readonly IPAddress _ipAddress = IPAddress.Parse("127.0.0.1");
        private readonly int _port = 8080;
        private readonly RecivedMessageHandler _recivedMessageHandler;
        public LoginWindow(RecivedMessageHandler recivedMessageHandler)
        {
            _recivedMessageHandler = recivedMessageHandler;
            InitializeComponent();
            _recivedMessageHandler.LoginSuccess += LoginSuccess;
            _recivedMessageHandler.LoginFailure += LoginFailure;
            Task.Run(async () =>
            {
                try
                {
                    await _recivedMessageHandler.mainViewModel.chatClient.ConnectAsync(_ipAddress, _port);
                    Debug.WriteLine("Connected to server.");
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await _recivedMessageHandler.RecivedCommandHandlerAsync(_recivedMessageHandler.mainViewModel.chatClient.ClientSocket);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Receive loop failed: {ex.Message}");
                            _recivedMessageHandler.mainViewModel.chatClient.Disconnect(); // Clean up on failure
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to connect to server: {ex.Message}");
                }
            }); 
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
            PasswordBox.Clear();
            UsernameBox.Clear();

            if (!_recivedMessageHandler.mainViewModel.chatClient.IsConnected)
            {
                var errorWindow = new ConnectionError("Unable to connect to the server. Please check your internet connection and try again."
)
                {
                    Owner = this // Make the popup modal and center it over LoginWindow
                };
                errorWindow.ShowDialog(); // Use ShowDialog for modal behavior
            }
            else if (username.Contains(" ") || password.Contains(" "))
            {
                var errorWindow = new ConnectionError("black-spaces are mot allowed")
                {
                    Owner = this
                };
                errorWindow.ShowDialog();
            }

            else
            {
                // Generate a temporary SenderId (e.g., Guid.Empty for unauthenticated users)
                string senderId = Guid.Empty.ToString(); // This should be replaced with a real user ID after login or registration

            // Create and execute the LoginCommand using the socket from IChatClient
            var loginCommand = new LoginCommand(username, password, senderId);
            await loginCommand.ExecuteAsync(_recivedMessageHandler.mainViewModel.chatClient.ClientSocket);
            }

        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_recivedMessageHandler);
            registerWindow.Show();
            this.Hide();  // Hide login window
        }

        public void LoginSuccess(object sender, string username)
        {
            try
            {
  var mainWindow = new MainWindow(_recivedMessageHandler.mainViewModel);
            mainWindow.Show();
            this.Hide();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
          
        }
        public void LoginFailure(object sender, string error)
        {
            var connectionError = new ConnectionError("Username or password are incorrect")
            {
                Owner = this
            };
            connectionError.ShowDialog();
        }
    }
}
