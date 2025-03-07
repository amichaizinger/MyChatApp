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
using System.Diagnostics;

namespace ChatAppSOLID.Services.Commands
{
    public class SendMessageCommand 
    {
        public string Content { get; }
        public string SenderId { get;  }
        public string? ReciverId { get; }
        public string? GroupId { get; }


        public SendMessageCommand(string content, string senderId, string? reciverId, string? groupId)
        {
            Content = content;
            SenderId = senderId;
            ReciverId = reciverId;
            GroupId = groupId;

        }

        public async Task<Message>ExecuteAsync(Socket clientSocket)
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

                return message;
            }

            catch (Exception)
            {
                Debug.WriteLine("message not sent");
                return null;
            }
        }
    }
}
