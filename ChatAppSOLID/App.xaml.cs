using System.Configuration;
using System.Data;
using System.Net.Sockets;
using System.Windows;
using ChatAppSOLID.Services.NewFolder;
using ChatAppSOLID.ViewModels;

namespace ChatAppSOLID
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly RecivedMessageHandler _recivedMessageHandler = new RecivedMessageHandler();


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            // Use the correct namespace based on your folder structure
            var loginwindow = new LoginWindow(_recivedMessageHandler);
            loginwindow.Show();
            this.Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            CloseSocketSafely();
        }

        public void CloseSocketSafely()
        {
            try
            {
                if (_recivedMessageHandler.mainViewModel.chatClient.ClientSocket != null && _recivedMessageHandler.mainViewModel.chatClient.ClientSocket.Connected)
                {
                    // First shutdown the socket
                    _recivedMessageHandler.mainViewModel.chatClient.ClientSocket.Shutdown(SocketShutdown.Both);
                    // Then close it
                    _recivedMessageHandler.mainViewModel.chatClient.ClientSocket.Close();
                    _recivedMessageHandler.mainViewModel.chatClient.ClientSocket.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing socket: {ex.Message}");
            }
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _recivedMessageHandler.mainViewModel.chatClient.Disconnect();
        }
    }

}
