using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Commands.interfaces;

namespace ChatApp.Server.Commands
{
    public class GeChatHistoryCommand
        : ICommand
    {
        public readonly Guid _senderId;
        private readonly List<Message> _chats;


        public GeChatHistoryCommand(Guid senderId, List<Message> messages)
        {
            _senderId = senderId;
            _chats = messages;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
           
            Message message = new Message
            {
                SenderId = _senderId,
                Content = JsonSerializer.Serialize(_chats),
                Command = CommandType.GetHistory,
                SentAt = DateTime.Now
            };
            string json = JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await clientSocket.SendAsync(buffer, SocketFlags.None);
        }
    } 
}
    