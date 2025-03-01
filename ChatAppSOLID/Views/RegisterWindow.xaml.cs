using System;
using System.Collections.Generic;
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
    public partial class RegisterWindow : Window
    {
        public bool IsPasswordValid = false;
        public bool IsUsernameValid = false;
        private readonly ChatClient _chatClient = new ChatClient();
        private readonly RecivedMessageHandler _recivedMessageHandler = new RecivedMessageHandler();

        public RegisterWindow()
        {
            InitializeComponent();
            _recivedMessageHandler.RegisterSuccess += RegisterSuccess;
            _recivedMessageHandler.RegisterFailure += RegisterFailure;   
        }

        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UsernameBox.Text.Length < 8)
            {
                UsernameBox.Background = Brushes.Red;
                RequirementBox.Visibility = Visibility.Visible;
                requierments.Visibility = Visibility.Visible;
                RegisterButton.IsEnabled = false;
            }
            else
            {
                UsernameBox.Background = Brushes.White; 
                IsUsernameValid = true;

                if (IsUsernameValid && IsPasswordValid)
                {
                    RequirementBox.Visibility = Visibility.Hidden;
                    requierments.Visibility = Visibility.Hidden;
                    RegisterButton.IsEnabled = true;

                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
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
                RequirementBox.Visibility = Visibility.Visible;
                requierments.Visibility = Visibility.Visible;
                PasswordBox.Background = Brushes.Red;
            }
            else
            {
                PasswordBox.Background = Brushes.White;
                IsPasswordValid = true;

                if (IsUsernameValid && IsPasswordValid)
                {
                    RequirementBox.Visibility = Visibility.Hidden;
                    requierments.Visibility = Visibility.Hidden;
                    RegisterButton.IsEnabled = true;  
                }
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (!_chatClient.IsConnected)
            {
                var errorWindow = new ConnectionError
                {
                    Owner = this
                };
                errorWindow.ShowDialog();
            }
            else
            {
                // Generate a temporary SenderId for registration
                Guid senderId = Guid.Empty;

                var registerCommand = new RegisterCommand(username, password, senderId); 
                await registerCommand.ExecuteAsync(_chatClient.ClientSocket);
            }
        }

        private void LoginLink_Click(object sender, RoutedEventArgs e)  
        {
            var loginWindow = new LoginWindow(); 
            loginWindow.Show();
            this.Hide();  // Hide register window
        }

        private void RegisterButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && RegisterButton.IsEnabled)
            {
                RegisterButton_Click(sender, e);
            }
        }
      
        public void RegisterSuccess(object sender, string username)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }
        public void RegisterFailure(object sender, string error)
        {
            var connectionError = new ConnectionError
            {
                Owner = this
            };
        }

    }
}
