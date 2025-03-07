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
using System.Windows;
using ChatAppSolid.Models.ChatAppSolid.Models;
using System.Windows.Controls;
using System.Text.Json;

namespace ChatAppSOLID.ViewModels
{
    public class MainViewModel : PropertyNotifier
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

        private string _userId;
        public string UserId
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
                FilterChats(); // Update filtered chats when all chats change
                RefreshUsers(); // Update users when all chats change
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

        private ObservableCollection<Chat> _filteredChats = new ObservableCollection<Chat>();
        public ObservableCollection<Chat> FilteredChats
        {
            get => _filteredChats;
            set
            {
                _filteredChats = value;
                OnPropertyChanged();
            }
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
                FilterChats(); // Call the filter method directly when search text changes
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

        private bool _isMenuOpen;
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set
            {
                _isMenuOpen = value;
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

        private string _newGroupName;
        public string NewGroupName
        {
            get => _newGroupName;
            set
            {
                _newGroupName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreateGroup)); // Update can create group when name changes
            }
        }

        private ObservableCollection<SelectableUser> _users = new ObservableCollection<SelectableUser>();
        public ObservableCollection<SelectableUser> Users
        {
            get => _users;
            private set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public List<SelectableUser> SelectedContacts
        {
            get => Users.Where(user => user.IsSelected).ToList();
        }

        private bool _isAboutPopupOpen;
        public bool IsAboutPopupOpen
        {
            get => _isAboutPopupOpen;
            set
            {
                _isAboutPopupOpen = value;
                OnPropertyChanged();
                if (!_isAboutPopupOpen)
                {
                    MediaState = MediaState.Stop; // Stop video when popup closes
                }
            }
        }

        private MediaState _mediaState;
        public MediaState MediaState
        {
            get => _mediaState;
            set
            {
                _mediaState = value;
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

        private ObservableCollection<string> _onlineUsers = new ObservableCollection<string>();
        public ObservableCollection<string> OnlineUsers
        {
            get => _onlineUsers;
            set
            {
                _onlineUsers = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Click Commands
        public ICommand SendCommand { get; }
        public ICommand CloseErrorPopupCommand { get; }
        public ICommand RefreshOnlineStatusCommand { get; }
        public ICommand AddToGroupCommand { get; }
        public ICommand LeaveGroupCommand { get; }
        public ICommand SeeParticipantsCommand { get; }
        public ICommand CloseGroupPopupCommand { get; }
        public ICommand OpenMenuCommand { get; }
        public ICommand OpenGroupPopupCommand { get; }
        public ICommand AboutDeveloperCommand { get; }
        public ICommand VisitWebsiteCommand { get; }
        public ICommand ConfirmCreateGroupCommand { get; }
        public ICommand CloseAboutPopupCommand { get; }
        public ICommand CloseParticipantsPopupCommand { get; }
        #endregion

        public MainViewModel()
        {
            // Initialize commands
            SendCommand = new RelayCommand(SendMessage);
            OpenGroupPopupCommand = new RelayCommand(OpenGroupPopup);
            VisitWebsiteCommand = new RelayCommand(VisitWebsite);
            CloseErrorPopupCommand = new RelayCommand(CloseErrorPopup);
            RefreshOnlineStatusCommand = new RelayCommand(RefreshOnlineStatus);
            AddToGroupCommand = new RelayCommand(AddToGroup);
            LeaveGroupCommand = new RelayCommand(LeaveGroup);
            SeeParticipantsCommand = new RelayCommand(OpenParticipantsPopup);
            CloseGroupPopupCommand = new RelayCommand(CloseGroupPopup);
            OpenMenuCommand = new RelayCommand(OpenMenu);
            AboutDeveloperCommand = new RelayCommand(OpenAboutPopup);
            ConfirmCreateGroupCommand = new RelayCommand(CreateGroup);
            CloseAboutPopupCommand = new RelayCommand(CloseAboutPopup);
            CloseParticipantsPopupCommand = new RelayCommand(CloseParticipantsPopup);

            // Initialize collections
            _users.Add(new SelectableUser(new User("kslfes", "fskenfsne")));
            Users = new ObservableCollection<SelectableUser>(_users);
            // Add initial chat
            AllChats.Add(new Chat("Best Chat App", null, new User { Id = "UserId" }, null)); // Private chat
            AllChats[0].AddMessage(new Message
            {
                Content = "Welcome to Best Chat App!" + 
                " 👋 Behold the GREATEST messaging app ever created! Our genius developers have crafted the most AMAZING chat experience known to humankind! You'll be BLOWN AWAY by how smooth and intuitive everything is!",
                Command = CommandType.SendMessage,
                SenderId = "UserId",
                ReciverId = UserId,
                SentAt = DateTime.Now,
            }, this);

            // Initialize filtered chats
            FilterChats();
        }

        #region Command Methods
        private async void SendMessage()
        {
            if (SelectedChat != null && !string.IsNullOrEmpty(MessageText))
            {
                SendMessageCommand sendCommand = new SendMessageCommand(
                    MessageText,
                    UserId,
                    SelectedChat.Friend?.Id,
                    SelectedChat.GroupId);

                try
                {
                    Message message = await sendCommand.ExecuteAsync(chatClient.ClientSocket);
                    SelectedChat.AddMessage(message, this);
                    MessageText = string.Empty;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred($"Failed to send message: {ex.Message}");
                }
            }
        }

        private void FilterChats()
        {
            if (_filteredChats == null)
                _filteredChats = new ObservableCollection<Chat>();

            _filteredChats.Clear();

            var chatsToShow = string.IsNullOrWhiteSpace(SearchText)
                ? AllChats
                : AllChats.Where(chat => chat.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);

            foreach (var chat in chatsToShow)
            {
                _filteredChats.Add(chat);
            }

            OnPropertyChanged(nameof(FilteredChats));
        }

        private void RefreshUsers()
        {
            // Store selected state to preserve it
            var selectedIds = Users
                .Where(u => u.IsSelected)
                .Select(u => u.User.Id)
                .ToList();

            // Clear and rebuild users collection
            _users.Clear();

            var usersToAdd = AllChats
                .Where(chat => chat.Friend != null)
                .Select(chat => new SelectableUser(chat.Friend))
                .ToList();

            foreach (var user in usersToAdd)
            {
                // Restore selected state
                if (selectedIds.Contains(user.User.Id))
                {
                    user.IsSelected = true;
                }
                _users.Add(user);
            }

            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(SelectedContacts));
            OnPropertyChanged(nameof(CanCreateGroup));
        }

        private void OpenGroupPopup()
        {
            IsGroupPopupOpen = true;
        }

        private void OpenAboutPopup()
        {
            IsAboutPopupOpen = true;
            MediaState = MediaState.Play; // Start video when popup opens
        }

        private void VisitWebsite()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo(
                "https://www.amazon.com/-/he/%D7%94%D7%99%D7%A8%D7%97-%D7%9E%D7%A0%D7%99%D7%95%D7%AA-%D7%97%D7%95%D7%9C%D7%A6%D7%AA-%D7%98%D7%A8%D7%99%D7%A7%D7%95-%D7%A9%D7%97%D7%95%D7%A8/dp/B08VG8QPV6/ref=sr_1_1?crid=KUKZTSALDYPQ&dib=eyJ2IjoiMSJ9.Lam5DsLPJeC2hGHqdzTCKzRKdSbcYdCbU4wxeRkRUJ98uyYgvoPIOM8WRtPloPY_A1FCcSO1TaA4Ry48Oc_P4vs0-rRhxBK98tyD20cJ2nB57MOfHj5ab7WkPekJ8G_f8q4WeSy2MBIF2D9NZz7IwZLhwwe61n3g2rUiNHrmFSX1IuHT5FTJ3US88mNhbNShU-AiJYKNZgbk0iTcvC1sqqS_YVRvq1vaHeeRYsXgBoA.qM7mO1j9JrG3EouPr_-GkNC7rvGCP53A8ofUq9x-iFI&dib_tag=se&keywords=amc+to+the+moon&qid=1741124849&sprefix=amc+to+the+moon%2Caps%2C221&sr=8-1")
            { UseShellExecute = true });
        }

        private void CloseErrorPopup()
        {
            IsErrorPopupOpen = false;
        }

        private void RefreshOnlineStatus()
        {
            // Placeholder: Implement logic to refresh online status
            OnErrorOccurred("Refreshing online status... (not implemented)");
        }

        private void AddToGroup()
        {
            if (SelectedChat != null && SelectedChat.GroupId != null)
            {
                // Placeholder: Implement add-to-group logic
                OnErrorOccurred("Add to group not implemented yet.");
            }
        }

        private void LeaveGroup()
        {
            if (SelectedChat != null && SelectedChat.GroupId != null)
            {
                // Placeholder: Implement leave-group logic
                OnErrorOccurred("Leave group not implemented yet.");
            }
        }

        private void OpenParticipantsPopup()
        {
            if (SelectedChat != null && SelectedChat.GroupId != null)
            {
                IsParticipantsPopupOpen = true;
            }
        }

        private void CloseGroupPopup()
        {
            IsGroupPopupOpen = false;

            // Reset selected state for all users
            foreach (var user in Users)
            {
                user.IsSelected = false;
            }

            // Reset group name
            NewGroupName = string.Empty;

            OnPropertyChanged(nameof(SelectedContacts));
            OnPropertyChanged(nameof(CanCreateGroup));
        }

        private void OpenMenu()
        {
            IsMenuOpen = true;
        }

        private void CreateGroup()
        {
            if (CanCreateGroup)
            {
                try
                {
                    var groupName = NewGroupName;
                    List<User> members = SelectedContacts.Select(su => su.User).ToList();
                    CreateGroupCommand createGroupCommand = new CreateGroupCommand(UserId, groupName, members);

                    // Here you would typically call ExecuteAsync and handle the result

                    IsGroupPopupOpen = false;

                    // Reset selected state for all users
                    foreach (var user in Users)
                    {
                        user.IsSelected = false;
                    }

                    // Reset group name
                    NewGroupName = string.Empty;

                    OnPropertyChanged(nameof(SelectedContacts));
                    OnPropertyChanged(nameof(CanCreateGroup));
                }
                catch (Exception ex)
                {
                    OnErrorOccurred($"Failed to create group: {ex.Message}");
                }
            }
        }

        public bool CanCreateGroup
        {
            get => SelectedContacts.Any() && !string.IsNullOrEmpty(NewGroupName);
        }

        private void CloseAboutPopup()
        {
            IsAboutPopupOpen = false;
        }

        private void CloseParticipantsPopup()
        {
            IsParticipantsPopupOpen = false;
        }
        #endregion

        #region Event Methods
        public void OnErrorOccurred(string errorMessage)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ErrorMessage = errorMessage;
                IsErrorPopupOpen = true;
            });
        }

        public void OnLoginSuccess(string username, string userId)
        {
            try
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Username = username;
                    UserId = userId;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                OnErrorOccurred($"Login error: {ex.Message}");
            }
        }

        public void OnChatHistoryReceived(List<Chat> chats)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var chat in chats)
                {
                    AllChats.Add(chat);
                }

                FilterChats();
                RefreshUsers();
            });
        }

        public void OnGroupCreated(Group group)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Chat chat = new Chat(
                    group.Name,
                    group.Id,
                    null,
                    new ObservableCollection<User>(group.Members));

                AllChats.Add(chat);
                FilterChats();
            });
        }

        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (!string.IsNullOrEmpty(message.GroupId))
                {
                    var groupChat = AllChats.FirstOrDefault(c => c.GroupId == message.GroupId);

                    if (groupChat != null)
                    {
                        groupChat.AddMessage(message, this);
                    }
                }
                else if (!string.IsNullOrEmpty(message.SenderId))
                {
                    var privateChat = AllChats.FirstOrDefault(
                        c => c.Friend?.Id == message.SenderId || c.Friend?.Id == message.ReciverId);

                    if (privateChat != null)
                    {
                        privateChat.AddMessage(message, this);
                    }
                }
            });
        }

        public void OnGroupLeft(string groupId)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var chat = AllChats.FirstOrDefault(c => c.GroupId == groupId);
                if (chat != null)
                {
                    AllChats.Remove(chat);
                    FilterChats();
                }
            });
        }

        public void OnOnlineUsersReceived(List<string> onlineUsers)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                OnlineUsers.Clear();
                foreach (var user in onlineUsers)
                {
                    OnlineUsers.Add(user);
                }
            });
        }

        public void OnAddedToGroup(List<string> usersId, string groupId)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var chat = AllChats.FirstOrDefault(c => c.GroupId == groupId);
                if (chat != null)
                {
                    foreach (var userId in usersId)
                    {
                        var userChat = AllChats
                            .Where(c => c.Friend != null)
                            .FirstOrDefault(c => c.Friend.Id == userId);

                        if (userChat?.Friend != null)
                        {
                            chat.AddParticipant(userChat.Friend);
                        }
                    }
                }
            });
        }

        public void OnNewUserReceived(User user)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Chat chat = new Chat(user.UserName, null, user, null);
                AllChats.Add(chat);
                FilterChats();
                RefreshUsers();
            });
        }
        #endregion
    }
}