using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.Interfaces;
using ChatAppSOLID.Services.NewFolder;
using ChatAppSOLID.Services.Commands.interfaces;
using System.Windows;
using System.Net.Sockets;
using System.Text.Json;
using System.Diagnostics;


namespace ChatAppSOLID.Services.Commands
{
    public class LeaveGroupCommand : ICommand
    {
        private readonly string _groupId;
        private readonly string _senderId;

        public LeaveGroupCommand(string groupId, string senderId)
        {
            _groupId = groupId;
            _senderId = senderId;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            var message = new Message
            {
                Command = CommandType.LeaveGroup,
                GroupId = _groupId,
                Content = " ",
                SenderId = _senderId,
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
