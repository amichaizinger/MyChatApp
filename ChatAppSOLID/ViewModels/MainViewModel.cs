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
using static ChatAppSOLID.ViewModels.SelectableUser;

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
                OnPropertyChanged(nameof(FilterChats)); 
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
            set
            {
                _filteredChats = value;
                OnPropertyChanged(nameof(FilteredChats)); 
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
                OnPropertyChanged(nameof(FilterChats)); // Notify UI that FilteredChats needs to update
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
                OnPropertyChanged(nameof(IsMenuOpen));
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
            }
        }

        public ObservableCollection<SelectableUser>?Users
        {
            get => new ObservableCollection<SelectableUser>(
                   AllChats.Where(chat => chat.Friend != null)
                          .Select(chat => chat.Friend)
                          .Select(user => new SelectableUser(user)));
        }

        private List<SelectableUser> _selectedContacts = new List<SelectableUser>();
        public List<SelectableUser> SelectedContacts
        {
            get => Users?.Where(user => user.IsSelected).ToList() ?? new List<SelectableUser>();
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
                OnPropertyChanged(nameof(MediaState));
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







        private ObservableCollection<string> _onlineUsers = new ObservableCollection<string>();
        public ObservableCollection<string> OnlineUsers
        {
            get => _onlineUsers;
            set { _onlineUsers = value; OnPropertyChanged(); }
        }



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
            FilteredChats = new ObservableCollection<Chat>(AllChats);

            _allChats.Add(new Chat("Best Chat App", null, null, null)); // Private chat
            _allChats[0].AddMessage(new Message
            {
                Content = "Welcome to Best Chat App! 👋 Behold the GREATEST messaging app ever created! Our genius developers have crafted the most AMAZING chat experience known to humankind! You'll be BLOWN AWAY by how smooth and intuitive everything is!",
                Command = CommandType.SendMessage,
                SenderId = UserId,
                SentAt = DateTime.Now,
            },this);

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
            // Load initial data asynchronously
        }



        #region Command Methods
        private async void SendMessage()
        {
            if (SelectedChat != null && !string.IsNullOrEmpty(MessageText))
            {

                SendMessageCommand sendCommand = new SendMessageCommand(MessageText, UserId, SelectedChat.Friend.Id, SelectedChat.GroupId);
                try
                {
                    Message message = await sendCommand.ExecuteAsync(chatClient.ClientSocket);
                    SelectedChat.Messages.Add(message);
                    MessageText = string.Empty;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred("Failed to send message.");
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
            IsAboutPopupOpen = true;
            MediaState = MediaState.Play; // Start video when popup opens
        }

        private void VisitWebsite()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.amazon.com/-/he/%D7%94%D7%99%D7%A8%D7%97-%D7%9E%D7%A0%D7%99%D7%95%D7%AA-%D7%97%D7%95%D7%9C%D7%A6%D7%AA-%D7%98%D7%A8%D7%99%D7%A7%D7%95-%D7%A9%D7%97%D7%95%D7%A8/dp/B08VG8QPV6/ref=sr_1_1?crid=KUKZTSALDYPQ&dib=eyJ2IjoiMSJ9.Lam5DsLPJeC2hGHqdzTCKzRKdSbcYdCbU4wxeRkRUJ98uyYgvoPIOM8WRtPloPY_A1FCcSO1TaA4Ry48Oc_P4vs0-rRhxBK98tyD20cJ2nB57MOfHj5ab7WkPekJ8G_f8q4WeSy2MBIF2D9NZz7IwZLhwwe61n3g2rUiNHrmFSX1IuHT5FTJ3US88mNhbNShU-AiJYKNZgbk0iTcvC1sqqS_YVRvq1vaHeeRYsXgBoA.qM7mO1j9JrG3EouPr_-GkNC7rvGCP53A8ofUq9x-iFI&dib_tag=se&keywords=amc+to+the+moon&qid=1741124849&sprefix=amc+to+the+moon%2Caps%2C221&sr=8-1") { UseShellExecute = true });
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
            IsGroupPopupOpen = false; 
            SelectedContacts.Clear();
        }

        private void OpenMenu()
        {
            IsMenuOpen = true; 
        }
        private void CreateGroup()
        {
            if (SelectedContacts.Any())
            {
                try
                {
                    var groupName = NewGroupName; // You can extend this to prompt the user for a name
                    List<User> members = SelectedContacts.Where(su => su.IsSelected).Select(su => su.User).ToList();
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

        public bool CanCreateGroup
        {
            get
            {
                if (SelectedContacts.Any() && !string.IsNullOrEmpty(NewGroupName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
            }

        }

        public void OnChatHistoryReceived(List<Chat> chats)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var chat in chats)
            {
                AllChats.Add(chat);
                    FilteredChats.Add(chat);
                }

            });

        }

        public void OnGroupCreated(Group group)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Chat chat = new Chat(group.Name, group.Id, null, new ObservableCollection<User>(group.Members));
            AllChats.Add(chat);
            });

        }
        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
         
            if (!string.IsNullOrEmpty(message.GroupId))
            {
                var groapChat = AllChats.FirstOrDefault(c => c.GroupId == message.GroupId);

                if (groapChat != null)
                {
                    groapChat.AddMessage(message, this);
                }
            }
            else if (!string.IsNullOrEmpty(message.ReciverId))
            {
                var privateChat = AllChats.FirstOrDefault(c => c.Friend.Id == message.SenderId || c.Friend.Id == message.ReciverId);

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
            }
            });
        }

        public void OnOnlineUsersReceived(List<string>? onlineUsers)
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

        public void OnAddedToGroup(List<string>usersId, string groupId)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
           var chat = AllChats.FirstOrDefault(c => c.GroupId == groupId);
                if (chat != null)
                {
                    foreach (var user in usersId)
                    {
                        var addedUser = AllChats.Where(chat => chat.Friend != null).FirstOrDefault(c => c.Friend.Id == user);
                        chat.AddParticipant(addedUser.Friend);
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
            });
        }


        #endregion



    }
}