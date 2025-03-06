using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.Commands.interfaces;

namespace ChatAppSOLID.Services.Commands
{
    internal class GetOnlineUsersCommand : ICommand
    {
        private readonly string _senderId;

        public GetOnlineUsersCommand(string senderId) 
        {
            _senderId = senderId;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            Message message = new Message
            {
                SenderId = _senderId,
                Content = " ",
                Command = CommandType.GetOnlineUsers,
                SentAt = DateTime.Now
            };
            try
            {
                // Convert Message object to JSON string
                string jsonMessage = JsonSerializer.Serialize(message);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
                await clientSocket.SendAsync(buffer, SocketFlags.None);

            }

            catch (Exception ex)
            {
                Debug.WriteLine("message not sent");
            }
        }
    }
}
