using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Commands;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Commands.interfaces;
using ChatApp.Server.Services.interfaces;


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
                Id = Guid.NewGuid().ToString(),
                Command = CommandType.LeaveGroup,
                GroupId = _groupId,
                SenderId = _senderId,
                SentAt = DateTime.Now
            };
            await SendWithLength.SendMessageAsync(clientSocket, message);
            ;
        }
    }
}
