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

        public int UnreadMessagesCount { get; set; }
       
        public bool IsGroup { get; set; }

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


        public Chat(string name, bool isGroup = false)
        {
            Messages = new ObservableCollection<Message>();
            UnreadMessagesCount = 0;
            Name = name;
            IsGroup = isGroup;
        }


        public void AddMessage(Message message)
        {

            Messages.Add(message);

            // Increment unread count if the message is received (not sent by the user)
            if (message.SenderId!=) //TODO: Add the user ID here
            {
                UnreadMessagesCount++;
            }
        }

        public void MarkAsRead()
        {
            UnreadMessagesCount = 0;
             }
    }
}
