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
using System.Diagnostics;
using System.Reflection.Metadata;


namespace ChatAppSOLID.Services.Commands
{
    public class AddUserToGroupCommand : ICommand
    {
        private readonly string _groupId;
        private readonly string _senderId;
        private string content;
        public List<User> Members;



        public AddUserToGroupCommand(string groupId, string senderId, List<User> members)
        {
            _groupId = groupId;
            _senderId = senderId;
            Members = members;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {

            var message = new Message
            {
                Command = CommandType.AddUserToGroup,
                SenderId = _senderId,
                Content = JsonSerializer.Serialize(Members),
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
                Debug.WriteLine("message not sent");
            }
        }
    }
}