using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatAppSOLID.Services.Commands.interfaces;
using ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.Interfaces;
using System.Net.Sockets;
using System.Text.Json;

namespace ChatAppSOLID.Services.Commands
{
    public class SendMessageCommand : ICommand
    {
        public string Content { get; }
        public Guid SenderId { get;  }
        public Guid? ReciverId { get; }
        public Guid? GroupId { get; }


        public SendMessageCommand(string content, Guid senderId, Guid? reciverId, Guid? groupId)
        {
            Content = content;
            SenderId = senderId;
            ReciverId = reciverId;
            GroupId = groupId;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            var message = new Message
            {
                Command = CommandType.SendMessage,
                Content = Content,
                SentAt = DateTime.Now,
                SenderId = SenderId,
                ReciverId = ReciverId,
                GroupId = GroupId

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
