using System.Collections.ObjectModel;
using ChatAppSOLID.Models;
using ChatAppSOLID.ViewModels;

namespace ChatAppSolid.Models
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using ChatAppSOLID.Models;
    using ChatAppSOLID.ViewModels;

    namespace ChatAppSolid.Models
    {
        public class Chat : PropertyNotifier
        {

            public string Name { get; set; }

            private ObservableCollection<Message> _messages;
            public ObservableCollection<Message> Messages
            {
                get => _messages;
                set
                {
                    _messages = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(LatestMessagePreview));
                    OnPropertyChanged(nameof(LatestMessageTime));
                    OnPropertyChanged(nameof(UnreadMessagesCount));
                }
            }

            private ObservableCollection<User>? _participents;
            public ObservableCollection<User>? Participants
            {
                get => _participents;
                set
                {
                    _participents = value;
                    OnPropertyChanged();
                }
            }

            private int _unread;
            public int UnreadMessagesCount
            {
                get => _unread;
                set
                {
                    _unread = value;
                    OnPropertyChanged(nameof(UnreadMessagesCount));
                    OnPropertyChanged(nameof(HasUnreadMessages));
                }
            }
            public bool HasUnreadMessages => UnreadMessagesCount > 0; // Computed property

            public User ?Friend { get; set; }
            public string? GroupId { get; set; }

            public string LatestMessagePreview
            {
                get
                {
                    if (Messages == null || !Messages.Any())
                    {
                        return string.Empty;
                    }
                    string latestMessage = Messages.Last().Content;
                    return latestMessage.Length > 50 ? latestMessage.Substring(0, 50) + "..." : latestMessage;
                }
            }

            public DateTime LatestMessageTime
            {
                get => Messages == null || !Messages.Any() ? DateTime.MinValue : Messages.Last().SentAt;
            }

            // Parameterless constructor for deserialization
            public Chat() { }

            // Original constructor for manual instantiation
            public Chat(string name, string? groupId, User? friend, ObservableCollection<User>? members)
            {
                Messages = new ObservableCollection<Message>();
                UnreadMessagesCount = 0;
                Participants = (members != null ? new ObservableCollection<User>(members) : new ObservableCollection<User>());
                Name = name;
                GroupId = groupId;
                Friend = friend;
            }


            public void AddMessage(Message message, MainViewModel mainViewModel)
            {
                Messages.Add(message);
                if (message.SenderId != mainViewModel.UserId)
                {
                    UnreadMessagesCount++;
                }
                OnPropertyChanged(nameof(Messages));
                OnPropertyChanged(nameof(UnreadMessagesCount));
                OnPropertyChanged(nameof(LatestMessageTime));
                OnPropertyChanged(nameof(LatestMessagePreview));
            }

            public void MarkAsRead() => UnreadMessagesCount = 0;

            public void AddParticipant(User user)
            {
                if (user != null && !string.IsNullOrEmpty(GroupId) && !Participants.Contains(user))
                    Participants.Add(user);
            }
        }
    }
}