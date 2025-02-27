using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppSOLID.Models
{
    public class Chat
    {
        public string Name { get; set; }

        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();  // Uses ObservableCollection for potential data binding (optional, can be IList if avoiding binding).
        public ObservableCollection<User> Participants { get; private set; } = new ObservableCollection<User>();
        public int UnreadMessagesCount { get; set; }

        public static Guid _currentUserId; // Store the current user's ID (static for simplicity, could be instance-based)

        public bool IsGroup { get; set; }

        public Guid Id { get; set; } 

        public string LatestMessagePreview
        {
            get
            {
                if (Messages == null || !Messages.Any())
                {
                    return string.Empty;
                }
                else
                {
                    string latestMessage = Messages.Last().Content;
                    if (latestMessage.Length > 50)
                    {
                        return latestMessage.Substring(0, 50) + "...";
                    }
                    else
                    {
                        return latestMessage;
                    }
                }
            }
        }


        public DateTime LatestMessageTime
        {
            get
            {
                if (Messages == null || !Messages.Any())
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return Messages.Last().SentAt;
                }
            }
        }


        public Chat(string name, bool isGroup)
        {
            Messages = new ObservableCollection<Message>();
            UnreadMessagesCount = 0;
            Participants = new ObservableCollection<User>();
            Name = name;
            IsGroup = isGroup;
        }

        public static void SetCurrentUserId(Guid userId)
        {
            _currentUserId = userId;
        }

        public static bool IsMessageSentByCurrentUser(Guid senderId)
        {
            return senderId == _currentUserId;
        }

        public void AddMessage(Message message)
        {

            Messages.Add(message);

            if (!IsMessageSentByCurrentUser(message.SenderId))
            {
                UnreadMessagesCount++;
            }
        }

        public void MarkAsRead()
        {
            UnreadMessagesCount = 0;
        }
        public void AddParticipant(User user)
        {
            if (user != null && IsGroup && !Participants.Contains(user))
            {
                Participants.Add(user);
            }
        }
    }
}
