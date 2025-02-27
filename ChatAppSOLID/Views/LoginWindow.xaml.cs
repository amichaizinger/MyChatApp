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

namespace ChatAppSOLID
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public bool IsPasswordValid = false;
        public bool IsUsernameValid = false;
        private readonly IChatClient _chtClient;
        private readonly IChatClient _chatClient = new ChatClient();

        public LoginWindow(IChatClient chatClient)
        {
            InitializeComponent();
            _chatClient = chatClient;
        }
       
        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UsernameBox.Text.Length < 8)
            {
                UsernameBox.Background = Brushes.Red;
            }
            else
            {
                PasswordBox.Background = Brushes.White;
                IsUsernameValid = true;

                if (IsUsernameValid && IsPasswordValid)
                {
                    requierments.Visibility = Visibility.Hidden;
                    LoginButton.IsEnabled = true;
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) //triggered whenever the password in the PasswordBox changes
        {
            if(PasswordBox.Password.Length < 8)
            {
                PasswordBox.Background = Brushes.Red;
            }
            else
            {
                PasswordBox.Background = Brushes.White;
                IsPasswordValid = true;

                if(IsUsernameValid && IsPasswordValid)
                {
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
    }
}
