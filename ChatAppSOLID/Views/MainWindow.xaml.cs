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
        public MainViewModel mainViewModel;
        public MainWindow(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            try // to check why mainwindow isnt responding
            {
                InitializeComponent();
                DataContext = mainViewModel;
                Debug.WriteLine("MainWindow DataContext set.");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}