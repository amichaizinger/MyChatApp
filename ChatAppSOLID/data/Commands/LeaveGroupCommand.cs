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


namespace ChatAppSOLID.Services.Commands
{
    public class LeaveGroupCommand : ICommand
    {
        private readonly Guid _groupId;
        private readonly Guid _senderId;

        public LeaveGroupCommand(Guid groupId, Guid senderId)
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
                SenderId = _senderId
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
                ErrorOccurred?.Invoke(this, ex.Message);
            }
        }
    }
}
