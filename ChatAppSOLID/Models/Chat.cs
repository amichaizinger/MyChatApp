using System.Collections.ObjectModel;
using ChatAppSOLID.Models;
using ChatAppSOLID.ViewModels;

namespace ChatAppSolid.Models
{
    public class Chat
    {
        private readonly MainViewModel _mainViewModel = new MainViewModel();  
        public string Name { get; set; }

        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();  // Uses ObservableCollection for potential data binding (optional, can be IList if avoiding binding).
        public ObservableCollection<User> ?Participants { get; private set; } = new ObservableCollection<User>();
        public int UnreadMessagesCount { get; set; }


        public Guid? FriendId { get; set; }
        public Guid? GroupId { get; set; }

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


        public Chat(string name, Guid ?groupId, Guid ?friendId, ObservableCollection<User> ?members)
        {
            Messages = new ObservableCollection<Message>();
            UnreadMessagesCount = 0;
            Participants = new ObservableCollection<User>(members);
            Name = name;
            GroupId = groupId;
            FriendId = friendId;
        }



        public void AddMessage(Message message)
        {

            Messages.Add(message);

            if (message.SenderId != _mainViewModel.UserId)
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
            if (user != null && GroupId.HasValue && !Participants.Contains(user))
            {
                Participants.Add(user);
            }
        }
    }
}