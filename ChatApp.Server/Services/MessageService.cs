using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static ChatApp.Server.Data.Database;

namespace ChatApp.Server.Services.NewFolder
{
    internal class MessageService : IMessageService
    {

        private readonly ChatDbContext _db;

        public MessageService()
        {
            _db = new ChatDbContext();
        }
        public async Task<bool> SaveMessageAsync(Message message)
        {
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<List<Chat>> GetChatHistoryAsync(Guid userId)
        {
            // Step 1: Fetch all relevant messages
            var allMessages = await _db.Messages
                .Where(m => m.SenderId == userId || m.ReciverId == userId ||
                            (m.GroupId.HasValue && _db.Groups.Any(g => g.Id == m.GroupId && g.Members.Any(u => u.Id == userId))))
                .Include(m => m.GroupId) // Include group details if applicable
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Step 2: Group messages into chats
            var chats = new List<Chat>();

            // Private chats: Group by the other user (sender or receiver)
            var privateMessages = allMessages.Where(m => !m.GroupId.HasValue);
            var privateChatGroups = privateMessages
                .GroupBy(m => m.SenderId == userId ? m.ReciverId : m.SenderId) // Key is the other user’s ID
                .Select(g => new Chat
                {
                    Id = g.Key, // Use the other user’s ID as the chat ID for private chats
                    Name = _db.Users.FirstOrDefault(u => u.Id == g.Key)?.Username ?? "Unknown User", // Fetch username
                    IsGroup = false,
                    Messages = new ObservableCollection<Message>(g.ToList()),
                    Participants = new ObservableCollection<User>
                    {
                        new User { Id = g.Key, Username = _db.Users.FirstOrDefault(u => u.Id == g.Key)?.Username ?? "Unknown" },
                        new User { Id = userId, Username = _db.Users.FirstOrDefault(u => u.Id == userId)?.Username ?? "You" }
                    }
                });

            chats.AddRange(privateChatGroups);

            // Group chats: Group by GroupId
            var groupMessages = allMessages.Where(m => m.GroupId.HasValue);
            var groupChatGroups = groupMessages
                .GroupBy(m => m.GroupId.Value)
                .Select(g => new Chat
                {
                    Id = g.Key,
                    Name = _db.Groups.FirstOrDefault(gr => gr.Id == g.Key)?.Name ?? "Unnamed Group",
                    IsGroup = true,
                    Messages = new ObservableCollection<Message>(g.ToList()),
                    Participants = new ObservableCollection<User>(
                        _db.Groups
                            .Where(gr => gr.Id == g.Key)
                            .SelectMany(gr => gr.Members)
                            .ToList())
                });

            chats.AddRange(groupChatGroups);

            // Step 3: Ensure no duplicate chats (e.g., if a participant has no messages yet)
            var distinctChats = chats
                .GroupBy(c => c.Id)
                .Select(g => g.First()) // Take the first instance of each chat ID
                .ToList();

            return distinctChats;
        }
    }
}
