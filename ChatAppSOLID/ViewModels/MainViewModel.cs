using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.NewFolder;
using Microsoft.Win32;

namespace ChatAppSOLID.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields and Properties

        // User Information
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private Guid _userId;
        public Guid UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        // Chat Management
        private ObservableCollection<Chat> _allChats = new ObservableCollection<Chat>();
        public ObservableCollection<Chat> AllChats
        {
            get => _allChats;
            set { _allChats = value; OnPropertyChanged(); }
        }

        private Chat _selectedChat;
        public Chat SelectedChat
        {
            get => _selectedChat;
            set
            {
                _selectedChat = value;
                OnPropertyChanged();
                if (_selectedChat != null)
                {
                    _selectedChat.UnreadMessagesCount = 0;
                }
            }
        }

        // Message Handling
        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set { _messageText = value; OnPropertyChanged(); }
        }

        // Group Creation
        private bool _isGroupPopupOpen;
        public bool IsGroupPopupOpen
        {
            get => _isGroupPopupOpen;
            set { _isGroupPopupOpen = value; OnPropertyChanged(); }
        }


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

        // Other UI Elements
        private ObservableCollection<string> _onlineUsers = new ObservableCollection<string>();
        public ObservableCollection<string> OnlineUsers
        {
            get => _onlineUsers;
            set { _onlineUsers = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand SendMessageCommand { get; }
        public ICommand CreateGroupCommand { get; }
        public ICommand CloseGroupPopupCommand { get; }
        public ICommand CreateNewGroupCommand { get; }
        public ICommand AboutDeveloperCommand { get; }
        public ICommand VisitWebsiteCommand { get; }
        public ICommand CloseErrorPopupCommand { get; }
        public ICommand AddToGroupCommand { get; }
        public ICommand LeaveGroupCommand { get; }
        public ICommand SeeParticipantsCommand { get; }

        #endregion


        private readonly RecivedMessageHandler _recivedMessageHandler;


        public MainViewModel()
        {
            // Initialize Commands
            SendMessageCommand = new RelayCommand(SendMessage, () => !string.IsNullOrEmpty(MessageText));
            CreateGroupCommand = new RelayCommand(() => IsGroupPopupOpen = true);
            CloseGroupPopupCommand = new RelayCommand(() => IsGroupPopupOpen = false);
            CreateNewGroupCommand = new RelayCommand(CreateNewGroup);
            AboutDeveloperCommand = new RelayCommand(ShowAboutDeveloper);
            VisitWebsiteCommand = new RelayCommand(VisitWebsite);
            CloseErrorPopupCommand = new RelayCommand(() => ErrorMessage = null);
            AddToGroupCommand = new RelayCommand(AddToGroup);
            LeaveGroupCommand = new RelayCommand(LeaveGroup);
            SeeParticipantsCommand = new RelayCommand(SeeParticipants);

            // Load initial data asynchronously
            LoadChatsAsync();
            LoadAvailableContactsAsync();
        }


        #region Methods

        private async void LoadHistoryAsync()
        {
            try
            {
                var chats = await _recivedMessageHandler.GetChatsAsync(UserId);
                foreach (var chat in chats)
                {
                    AllChats.Add(chat);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load chats: {ex.Message}";
            }
        }

        private async void LoadAvailableContactsAsync()
        {
            try
            {
                var contacts = await _recivedMessageHandler.GetContactsAsync(UserId);
                foreach (var contact in contacts)
                {
                    AvailableContacts.Add(contact);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load contacts: {ex.Message}";
            }
        }

        private async void SendMessage()
        {
            if (SelectedChat != null && !string.IsNullOrWhiteSpace(MessageText))
            {
                try
                {
                    var message = new Message
                    {
                        Content = MessageText,
                        SenderId = UserId,
                        SentAt = DateTime.Now
                    };
                    SelectedChat.Messages.Add(message);
                    await _recivedMessageHandler.SendMessageAsync(SelectedChat.Id, message);
                    MessageText = string.Empty;
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to send message: {ex.Message}";
                }
            }
        }

        private async void CreateNewGroup()
        {
            if (SelectedContacts.Any())
            {
                try
                {
                    var groupName = "New Group"; // You can extend this to prompt the user for a name
                    var memberIds = SelectedContacts.Select(c => c.Id).ToList();
                    var newGroup = await _recivedMessageHandler.CreateGroupAsync(groupName, memberIds);
                    AllChats.Add(newGroup);
                    IsGroupPopupOpen = false;
                    SelectedContacts.Clear();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to create group: {ex.Message}";
                }
            }
        }

        private void ShowAboutDeveloper()
        {
            // Placeholder: Add logic to display a popup with developer info
            ErrorMessage = "About Developer: [Your Name], [Your User Info]";
        }

        private void VisitWebsite()
        {
            // Placeholder: Add logic to open a website (e.g., using Process.Start)
            ErrorMessage = "Website feature not implemented yet.";
        }

        private void AddToGroup()
        {
            // Placeholder: Add logic to add a contact to an existing group
            if (SelectedChat != null)
            {
                ErrorMessage = "Add to group feature not fully implemented.";
            }
        }

        private void LeaveGroup()
        {
            // Placeholder: Add logic to leave the selected group
            if (SelectedChat != null)
            {
                ErrorMessage = "Leave group feature not fully implemented.";
            }
        }

        private void SeeParticipants()
        {
            // Placeholder: Add logic to display group participants
            if (SelectedChat != null)
            {
                ErrorMessage = "See participants feature not fully implemented.";
            }
        }
        
        public void OnLoginSuccess(string username, Guid userId)
        {
            Username = username;
            UserId = userId;
            LoadChatsAsync();
            LoadAvailableContactsAsync();
        }

        public void OnHistoryReceived(List<Chat> chats)
        {
            foreach (var chat in chats)
            {
                AllChats.Add(chat);
            }
        }

        public void OnGroupCreated(string groupName, List<Guid> members, Guid groupId)
        {
            var newGroup = new Chat(groupName, true)
            {
                Id = groupId,
                Members = new ObservableCollection<User>(members.Select(m => new User { Id = m, Username = "Unknown User" })),
                Messages = new ObservableCollection<Message>()
            };
            AllChats.Add(newGroup);
        }
        public void OnMessageReceived(Message message)
        {
            var chat = AllChats.FirstOrDefault(c => c.Id == (message.GroupId.HasValue ? message.GroupId.Value : message.ReciverId)); if (chat != null)
            {
                chat.AddMessage(message);
            }
        }
        public void OnError(string error)
        {
            ErrorMessage = error;
        }



        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}

