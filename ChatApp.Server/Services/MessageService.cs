using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using static ChatApp.Server.Data.Database;

namespace ChatApp.Server.Services.NewFolder
{
    internal class MessageService : IMessageService
    {

        private readonly ChatDbContext _db;

        public MessageService()
        {
            _db = Program.ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ChatDbContext>();
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

            // Step 2: Fetch all groups the user is part of (with or without messages)
            var userGroups = await _db.Groups
                .Where(g => g.Members.Any(u => u.Id == userId))
                .Include(g => g.Members)
                .ToListAsync();

            // Step 3: Fetch all registered users (for potential private chats)
            var allUsers = await _db.Users
                .Where(u => u.Id != userId) // Exclude the current user
                .ToListAsync();

            // Step 4: Build the list of chats
            var chats = new List<Chat>();

            // 4.1: Private chats from messages (group by the other user)            var chats = new List<Chat>();
            var privateMessages = allMessages.Where(m => !m.GroupId.HasValue);
            var privateChatGroups = privateMessages
                .GroupBy(m => m.SenderId == userId ? m.ReciverId : m.SenderId) // Key is the other user’s ID
                .Select(g => new Chat
                {
                    Name = _db.Users.FirstOrDefault(u => u.Id == g.Key)?.UserName ?? "Unknown User", // Fetch username
                    GroupId = null,
                    FriendId = g.Key, // Use the other user’s ID as the chat ID for private chats
                    Participants = null,
                    Messages = new ObservableCollection<Message>(g.ToList())
                });

            chats.AddRange(privateChatGroups);

            // 4.2: Group chats from messages (group by GroupId)
            var groupMessages = allMessages.Where(m => m.GroupId.HasValue);
            var groupChatGroups = groupMessages
                .GroupBy(m => m.GroupId.Value)
                .Select(g => new Chat
                {
                    Name = _db.Groups.FirstOrDefault(gr => gr.Id == g.Key)?.Name ?? "Unnamed Group",
                    GroupId = g.Key,
                    FriendId = null,
                    Participants = new ObservableCollection<User>(
                        _db.Groups
                            .Where(gr => gr.Id == g.Key)
                            .SelectMany(gr => gr.Members)
                            .ToList()),
                    Messages = new ObservableCollection<Message>(g.ToList())
                });

            chats.AddRange(groupChatGroups);

            //Step 4.3 add empty chats
            var remainingGroups = userGroups.Where(g => !chats.Any(c => c.GroupId == g.Id));
            chats.AddRange(remainingGroups.Select(g => new Chat
            {
                Name = g.Name,
                GroupId = g.Id,
                FriendId = null,
                Participants = new ObservableCollection<User>(g.Members),
                Messages = new ObservableCollection<Message>()
            }));
            var remainingUsers = allUsers.Where(u => !chats.Any(c => c.FriendId == u.Id));
            chats.AddRange(remainingUsers.Select(u => new Chat
            {
                Name = u.UserName,
                GroupId = null,
                FriendId = u.Id,
                Participants = null,
                Messages = new ObservableCollection<Message>()
            }));
            // Step 5: Ensure no duplicate chats (e.g., if a participant has no messages yet)
            var distinctChats = chats
                .GroupBy(c => (c.FriendId.HasValue ? c.FriendId : c.GroupId))
                .Select(g => g.First()) // Take the first instance of each chat ID
                .ToList();

            return distinctChats;
        }
    }
}
