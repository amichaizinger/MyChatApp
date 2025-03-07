using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.Commands.interfaces;
using ChatAppSOLID.Services.Interfaces;

namespace ChatAppSOLID.Services.Commands
{
    public class CreateGroupCommand : ICommand
    {
        public string SenderId { get; }
        public List<User> Members { get; }
        public string Name { get; }

        public CreateGroupCommand(string senderId, string name, List<User> members)
        {
            Members = members;
            Name = name;
            SenderId = senderId;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            var group = new Group
            {
                Id = Guid.NewGuid().ToString(),
                Members = this.Members,
                Name = this.Name,
                Messages = new List<Message>()
            };
            Message message = new Message
            {
                SenderId = SenderId,
                Content =JsonSerializer.Serialize(group),
                Command = CommandType.CreateGroup,
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
