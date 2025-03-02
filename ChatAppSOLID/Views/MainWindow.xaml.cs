using ChatAppSOLID.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace ChatAppSOLID
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Debug.WriteLine("MainWindow InitializeComponent completed.");

                var _viewModel = new MainViewModel();
                DataContext = _viewModel;
                Debug.WriteLine("MainWindow DataContext set.");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MainWindow initialization failed: {ex.Message}");
                MessageBox.Show($"Failed to initialize MainWindow: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}