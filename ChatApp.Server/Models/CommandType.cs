using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Server.Models
{
        public enum CommandType
        {
            Login,              // Authenticate a user
            Register,           // Create a new user
            SendMessage,        // Send a private or group message
            CreateGroup,        // Create a new group
            LeaveGroup,         // Leave a group
            GetOnlineUsers,     // Retrieve list of online users
            AddUserToGroup,      // Add another user to a group
        GetChatHistory      // Retrieve chat history

    }
}

