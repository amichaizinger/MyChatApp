using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Commands.interfaces;
using ChatApp.Server.Services.interfaces;


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
                SenderId = _senderId,
                SentAt = DateTime.Now
            };
            string json = JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await clientSocket.SendAsync(buffer, SocketFlags.None);
        }
    }
}
