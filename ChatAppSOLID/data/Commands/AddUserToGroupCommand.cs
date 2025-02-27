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
using System.Net.Sockets;
using System.Text.Json;


namespace ChatAppSOLID.Services.Commands
{
    public class AddUserToGroupCommand : ICommand
    {
        private readonly Guid _groupId;
        private readonly Guid _senderId;
        private string content;
        public List<Guid> Members;



        public AddUserToGroupCommand(Guid groupId, Guid senderId, List<Guid> members)
        {
            _groupId = groupId;
            _senderId = senderId;
            Members = members;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
             content = " ";

            foreach (var member in Members)
            {
                content += member + " ";
            }

            var message = new Message
            {
                Command = CommandType.AddUserToGroup,
                SenderId = _senderId,
                Content = content,
                GroupId = _groupId
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