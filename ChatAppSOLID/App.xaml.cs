using System.Configuration;
using System.Data;
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
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _recivedMessageHandler.mainViewModel.chatClient.Disconnect();
        }
    }

}
