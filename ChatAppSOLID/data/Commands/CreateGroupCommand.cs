using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
        public Guid SenderId { get; }
        public List<Guid> Members { get; }
        public string Name { get; }

        public CreateGroupCommand( Guid senderId, string name, List<Guid> members)
        {
            Members = members;
            Name = name;
            SenderId = senderId;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            string content = Name + " ";

            foreach (var member in Members)
            {
               content += member + " ";
            }
            Message message = new Message
            {
                SenderId = SenderId,
                Content = content,
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
                ErrorOccurred?.Invoke(this, ex.Message);
            }
        }
    }
}
