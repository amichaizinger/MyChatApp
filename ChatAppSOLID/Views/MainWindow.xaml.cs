using ChatAppSOLID.ViewModels;
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
            InitializeComponent();
            DataContext = new MainViewModel(); // Ensure correct namespace
        }
        private void CloseErrorPopup_Click(object sender, RoutedEventArgs e)
        {
            errorPopup.IsOpen = false;
        }

        private void CreateNewGroup_Click(object sender, RoutedEventArgs e)
        {
            createGroupPopup.IsOpen = true;
        }

        private void AboutDeveloper_Click(object sender, RoutedEventArgs e)
        {
            aboutDeveloperPopup.IsCloseOpen = true;
        }

        private void VisitWebsite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://yourdomain.com",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ShowError("Could not open website: " + ex.Message);
            }
        }

        private void AddToGroup_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Handle adding the selected contact to a group
            // Show group selection interface or create new group
            createGroupPopup.IsOpen = true;
        }

        private void LeaveGroup_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Handle leaving the current group
        }

        private void SeeParticipants_Click(object sender, RoutedEventArgs e)
        {
            participantsPopup.IsOpen = true;
        }

        private void CloseGroupPopup_Click(object sender, RoutedEventArgs e)
        {
            createGroupPopup.IsOpen = false;
        }

        private void GroupSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Filter contacts in the group creation popup
        }

        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Create a new group with selected contacts
            createGroupPopup.IsOpen = false;
        }

        private void CloseAboutPopup_Click(object sender, RoutedEventArgs e)
        {
            aboutDeveloperPopup.IsOpen = false;
        }

        private void CloseParticipantsPopup_Click(object sender, RoutedEventArgs e)
        {
            participantsPopup.IsOpen = false;
        }

        public void ShowError(string errorMessage)
        {
            errorMessageText.Text = errorMessage;
            errorPopup.IsOpen = true;
        }
    }
}