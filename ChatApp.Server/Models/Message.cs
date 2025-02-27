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
        public Guid Id { get; } = Guid.NewGuid();
        public MessageStatus Status { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public Guid SenderId { get; set; }
        public Guid? ReciverId { get; set; }  // null if it's a group message
        public Guid? GroupId { get; set; }     // null if it's a private message
        
    }
}
