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
        public class Chat 
        {

            public string Name { get; set; }
            public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>(); // Public setter
            public ObservableCollection<User>? Participants { get; set; } = new ObservableCollection<User>(); // Public setter
            public int UnreadMessagesCount { get; set; }
            public User? Friend { get; set; }
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
                if (message.SenderId != mainViewModel.UserId) UnreadMessagesCount++;
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