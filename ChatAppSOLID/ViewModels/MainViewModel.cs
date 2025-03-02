using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.Commands;
using ChatAppSOLID.Services.NewFolder;
using System.Diagnostics;
using System.Net.Sockets;

namespace ChatAppSOLID.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged

    {
        public ChatClient chatClient = new ChatClient();


        #region Properties
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private Guid _userId;
        public Guid UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Chat> _allChats = new ObservableCollection<Chat>();
        public ObservableCollection<Chat> AllChats
        {
            get => _allChats;
            set
            {
                _allChats = value;
                OnPropertyChanged();
                FilterChats(); // Update FilteredChats when AllChats changes
            }
        }

        private Chat _selectedChat;
        public Chat SelectedChat
        {
            get => _selectedChat;
            set
            {
                _selectedChat = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Chat> _filteredChats;
        public ObservableCollection<Chat> FilteredChats
        {
            get => _filteredChats;
            set { _filteredChats = value; OnPropertyChanged(); }
        }

        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPropertyChanged();
            }
        }

        private string _groupSearchText;
        public string GroupSearchText
        {
            get => _groupSearchText;
            set
            {
                _groupSearchText = value;
                OnPropertyChanged();
                
            }
        }
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterChats(); // Update FilteredChats when SearchText changes
            }
        }

        private bool _isErrorPopupOpen;
        public bool IsErrorPopupOpen
        {
            get => _isErrorPopupOpen;
            set
            {
                _isErrorPopupOpen = value;
                OnPropertyChanged();
            }
        }

        private bool _isGroupPopupOpen;
        public bool IsGroupPopupOpen
        {
            get => _isGroupPopupOpen;
            set
            {
                _isGroupPopupOpen = value;
                OnPropertyChanged();
            }
        }

        private bool _isAboutPopupOpen;
        public bool IsAboutPopupOpen
        {
            get => _isAboutPopupOpen;
            set
            {
                _isAboutPopupOpen = value;
                OnPropertyChanged();
            }
        }

        private bool _isParticipantsPopupOpen;
        public bool IsParticipantsPopupOpen
        {
            get => _isParticipantsPopupOpen;
            set
            {
                _isParticipantsPopupOpen = value;
                OnPropertyChanged();
            }
        }
        #endregion






        private ObservableCollection<Models.User> _availableContacts = new ObservableCollection<User>();
        public ObservableCollection<Models.User> AvailableContacts
        {
            get => _availableContacts;
            set { _availableContacts = value; OnPropertyChanged(); }
        }

        private ObservableCollection<User> _selectedContacts = new ObservableCollection<User>();
        public ObservableCollection<User> SelectedContacts
        {
            get => _selectedContacts;
            set { _selectedContacts = value; OnPropertyChanged(); }
        }

        private ObservableCollection<User> _onlineUsers = new ObservableCollection<User>();
        public ObservableCollection<User> OnlineUsers
        {
            get => _onlineUsers;
            set { _onlineUsers = value; OnPropertyChanged(); }
        }



        #region Click Commands
        public ICommand SendCommand { get; }
        public ICommand CreateNewGroupCommand { get; }
        public ICommand AboutDeveloperCommand { get; }
        public ICommand VisitWebsiteCommand { get; }
        public ICommand CloseErrorPopupCommand { get; }
        public ICommand RefreshOnlineStatusCommand { get; }
        public ICommand AddToGroupCommand { get; }
        public ICommand LeaveGroupCommand { get; }
        public ICommand SeeParticipantsCommand { get; }
        public ICommand CloseGroupPopupCommand { get; }
        public ICommand OnGroupSearchTextCommand { get; }
        public ICommand CreateGroupCommand { get; }
        public ICommand CloseAboutPopupCommand { get; }
        public ICommand CloseParticipantsPopupCommand { get; }
        #endregion



        public MainViewModel()
        {
            SendCommand = new RelayCommand(SendMessage, () => !string.IsNullOrEmpty(MessageText));
            CreateNewGroupCommand = new RelayCommand(OpenGroupPopup);
            AboutDeveloperCommand = new RelayCommand(OpenAboutPopup);
            VisitWebsiteCommand = new RelayCommand(VisitWebsite);
            CloseErrorPopupCommand = new RelayCommand(CloseErrorPopup);
            RefreshOnlineStatusCommand = new RelayCommand(RefreshOnlineStatus);
            AddToGroupCommand = new RelayCommand(AddToGroup);
            LeaveGroupCommand = new RelayCommand(LeaveGroup);
            SeeParticipantsCommand = new RelayCommand(OpenParticipantsPopup);
            CloseGroupPopupCommand = new RelayCommand(CloseGroupPopup);
            OnGroupSearchTextCommand = new RelayCommand(OnGroupSearchText);
            CreateGroupCommand = new RelayCommand(CreateGroup);
            CloseAboutPopupCommand = new RelayCommand(CloseAboutPopup);
            CloseParticipantsPopupCommand = new RelayCommand(CloseParticipantsPopup);
            // Load initial data asynchronously
        }

       

        #region Command Methods
        private async void SendMessage()
        {
            if (SelectedChat != null && !string.IsNullOrEmpty(MessageText))
            {

                SendMessageCommand sendCommand = new SendMessageCommand(MessageText, UserId, SelectedChat.FriendId, SelectedChat.GroupId);
                Message message = await sendCommand.ExecuteAsync(chatClient.ClientSocket);
                if (message != null)
                {
                    SelectedChat.Messages.Add(message);
                    MessageText = string.Empty;
                }
                else
                {
                    ErrorMessage = "Failed to send message.";
                }
            }
        }

        private void FilterChats()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredChats = new ObservableCollection<Chat>(AllChats);
            }
            else
            {
                var filtered = AllChats
                    .Where(chat => chat.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
                FilteredChats = new ObservableCollection<Chat>(filtered);
            }
        }

        private void OpenGroupPopup()
        {
            IsGroupPopupOpen = true; // Opens the group creation popup
        }

        private void OpenAboutPopup()
        {
            IsAboutPopupOpen = true; // Opens the "About Developer" popup
        }

        private void VisitWebsite()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://example.com") { UseShellExecute = true });
            // No UI element opened or closed
        }

        private void CloseErrorPopup()
        {
            IsErrorPopupOpen = false; // Closes the error notification popup
        }

        private void RefreshOnlineStatus()
        {
            // Placeholder: Implement logic to refresh online status
            ErrorMessage = "Refreshing online status... (not implemented)";
            // No UI element opened or closed
        }

        private void AddToGroup()
        {
            if (SelectedChat != null && SelectedChat.GroupId != null)
            {
                // Placeholder: Implement add-to-group logic
                ErrorMessage = "Add to group not implemented yet.";
            }
            // No UI element opened or closed directly
        }

        private void LeaveGroup()
        {
            if (SelectedChat != null && SelectedChat.GroupId != null)
            {
                // Placeholder: Implement leave-group logic
                ErrorMessage = "Leave group not implemented yet.";
            }
            // No UI element opened or closed directly
        }

        private void OpenParticipantsPopup()
        {
            if (SelectedChat != null && SelectedChat.GroupId != null)
            {
                IsParticipantsPopupOpen = true; // Opens the participants popup
            }
        }

        private void CloseGroupPopup()
        {
            IsGroupPopupOpen = false; // Closes the group creation popup
        }

        private void OnGroupSearchText()
        {
            throw new NotImplementedException();
        }
        private void CreateGroup()
        {
            // Placeholder: Implement group creation logic (e.g., send to server)
            ErrorMessage = "Group created (not fully implemented).";
            IsGroupPopupOpen = false; // Closes the group creation popup after creation
        }

        private void CloseAboutPopup()
        {
            IsAboutPopupOpen = false; // Closes the "About Developer" popup
        }

        private void CloseParticipantsPopup()
        {
            IsParticipantsPopupOpen = false; // Closes the participants popup
        }
        #endregion




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }








        #region Methods

        public void OnErrorOccurred(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsErrorPopupOpen = true;
        }



        private async void CreateNewGroup()
        {
            if (SelectedContacts.Any())
            {
                try
                {
                    var groupName = "New Group"; // You can extend this to prompt the user for a name
                    List<User> members = SelectedContacts.ToList();
                    CreateGroupCommand createGroupCommand = new CreateGroupCommand(UserId, groupName, members);

                    IsGroupPopupOpen = false;
                    SelectedContacts.Clear();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to create group: {ex.Message}";
                }
            }
        }





        public void OnLoginSuccess(string username, Guid userId)
        {
            Username = username;
            UserId = userId;
        }

        public void OnChatHistoryReceived(List<Chat> chats)
        {
            foreach (var chat in chats)
            {
                AllChats.Add(chat);
            }
        }

        public void OnGroupCreated(Group group)
        {
            Chat chat = new Chat(group.Name, group.Id, null, new ObservableCollection<User>(group.Members));
            AllChats.Add(chat);
        }
        public void OnMessageReceived(Message message)
        {
            if (message.GroupId.HasValue)
            {
                var groapChat = AllChats.FirstOrDefault(c => c.GroupId == message.GroupId);

                if (groapChat != null)
                {
                    groapChat.AddMessage(message);
                }
            }
            else if (message.ReciverId.HasValue)
            {
                var privateChat = AllChats.FirstOrDefault(c => c.FriendId == message.SenderId || c.FriendId == message.ReciverId);

                if (privateChat != null)
                {
                    privateChat.AddMessage(message);
                }
            }
        }

        public void OnGroupLeft(Guid groupId)
        {
            var chat = AllChats.FirstOrDefault(c => c.GroupId == groupId);
            if (chat != null)
            {
                AllChats.Remove(chat);
            }
        }

        public void OnOnlineUsersReceived(List<User>? onlineUsers)
        {
            OnlineUsers.Clear();
            foreach (var user in onlineUsers)
            {
                OnlineUsers.Add(user);
            }
        }

        public void OnAddedToGroup(Group group)
        {

            Chat chat = new Chat(group.Name, group.Id, null, new ObservableCollection<User>(group.Members));
            AllChats.Add(chat);
        }

        public void OnNewUserReceived(User user)
        {
            Chat chat = new Chat(user.UserName, null, user.Id, null);
            AllChats.Add(chat);
        }


        #endregion



    }
}