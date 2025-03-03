﻿using System;
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

        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();  // Uses ObservableCollection for potential data binding (optional, can be IList if avoiding binding).
        public ObservableCollection<User>? Participants { get; set; } = new ObservableCollection<User>();
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


        public Chat(string name, Guid? groupId, Guid? friendId, ObservableCollection<User>? members, ObservableCollection<Message> ?messages)
        {
            Messages = messages;
            Participants = new ObservableCollection<User>(members);
            Name = name;
            GroupId = groupId;
            FriendId = friendId;
        }

        
        public Chat()
        {
        }



    }
}
