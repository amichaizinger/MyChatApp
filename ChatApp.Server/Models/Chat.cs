using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Server.Models
{
    public class Chat
    {
        public string Name { get; set; }

        public ObservableCollection<Message> Messages { get; set; } 
        public ObservableCollection<User>? Participants { get; set; } 
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


        public Chat(string name, string? groupId, User? friend, ObservableCollection<User>? members, ObservableCollection<Message> ?messages)
        {
            Messages = messages;
            Participants = new ObservableCollection<User>(members);
            Name = name;
            GroupId = groupId;
            Friend = friend;
        }

        
        public Chat()
        {
        }



    }
}
