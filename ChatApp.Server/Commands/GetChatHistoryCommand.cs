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
    public class GetChatHistoryCommand
        : ICommand
    {
        public readonly string _senderId;
        private readonly List<Chat> _chats;


        public GetChatHistoryCommand(string senderId, List<Chat> chats)
        {
            _senderId = senderId;
            _chats = chats;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            List<Chat> currentBatch = new List<Chat>();
            int estimatedTotalSize = 0;

            foreach (var chat in _chats)
            {
                string chatJson = JsonSerializer.Serialize(chat);
                int chatNumBytes = Encoding.UTF8.GetByteCount(chatJson);
                if (estimatedTotalSize + chatNumBytes < 900)
                {
                    estimatedTotalSize += chatNumBytes;
                    currentBatch.Add(chat);
                }
                else
                {
                    await SendBatchAsync(clientSocket, currentBatch);
                    currentBatch.Clear();
                    estimatedTotalSize = chatNumBytes;
                    currentBatch.Add(chat);
                }
            }

            if (currentBatch.Count > 0)
            {
                await SendBatchAsync(clientSocket, currentBatch);
            }
        }

        private async Task SendBatchAsync(Socket clientSocket, List<Chat> batch)
        {
            try
            {
                Message message = new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderId = _senderId,
                    Content = JsonSerializer.Serialize(batch),
                    Command = CommandType.GetChatHistory,
                    SentAt = DateTime.Now
                };
                await SendWithLength.SendMessageAsync(clientSocket, message);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending batch: {ex.Message}");
            }
        }
    }
}  