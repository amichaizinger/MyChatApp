using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Models;

namespace ChatApp.Server.Models
{
    public class Message
    {
        public CommandType Command { get; set; }
        public string Id { get; set;  } = Guid.NewGuid().ToString();
        public MessageStatus Status { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public string SenderId { get; set; }
        public string? ReciverId { get; set; }  // null if it's a group message
        public string? GroupId { get; set; }     // null if it's a private message
        
    }
}
