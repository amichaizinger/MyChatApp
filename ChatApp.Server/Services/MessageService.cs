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

        public async Task<List<Chat>> GetChatHistoryAsync(string userId)
        {
            // Step 1: Prepare collections to store our results
            var chats = new List<Chat>();

            // Step 2: Get all users (excluding current user) and create a lookup
            var allUsers = await _db.Users
                .Where(u => u.Id != userId)
                .ToDictionaryAsync(u => u.Id, u => u);

            // Step 3: Fetch all groups the user is part of with their members
            var userGroups = await _db.Groups
                .Where(g => g.Members.Any(u => u.Id == userId))
                .Include(g => g.Members)
                .ToDictionaryAsync(g => g.Id, g => g);

            // Step 4: Fetch all relevant messages with proper includes
            var allMessages = await _db.Messages
                .Where(m => (m.SenderId == userId || m.ReciverId == userId) && string.IsNullOrEmpty(m.GroupId) ||
                            (!string.IsNullOrEmpty(m.GroupId) && _db.Groups.Any(g => g.Id == m.GroupId && g.Members.Any(u => u.Id == userId))))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Step 5: Process private chats (direct messages) - FIXED LOGIC
            // First separate direct messages
            var privateMessages = allMessages.Where(m => string.IsNullOrEmpty(m.GroupId)).ToList();

            // Create dictionary to store user ID to message list mapping
            var conversationsByUser = new Dictionary<string, List<Message>>();

            // Loop through all private messages and organize them by conversation partner
            foreach (var message in privateMessages)
            {
                // Determine who the conversation partner is
                string partnerId;
                if (message.SenderId == userId)
                {
                    partnerId = message.ReciverId;
                }
                else // message.ReciverId == userId
                {
                    partnerId = message.SenderId;
                }

                // Make sure we have a list for this partner
                if (!conversationsByUser.ContainsKey(partnerId))
                {
                    conversationsByUser[partnerId] = new List<Message>();
                }

                // Add this message to the appropriate conversation
                conversationsByUser[partnerId].Add(message);
            }

            // Now create a chat for each unique conversation partner
            foreach (var entry in conversationsByUser)
            {
                var partnerId = entry.Key;
                var messages = entry.Value;

                string partnerName = "Unknown User";
                User partner = null;

                if (allUsers.TryGetValue(partnerId, out var foundUser))
                {
                    partnerName = foundUser.UserName;
                    partner = foundUser;
                }

                chats.Add(new Chat
                {
                    Name = partnerName,
                    GroupId = null,
                    Friend = partner,
                    Participants = null,
                    Messages = new ObservableCollection<Message>(messages)
                });
            }

            // Step 6: Process group chats
            var groupMessages = allMessages.Where(m => !string.IsNullOrEmpty(m.GroupId))
                .GroupBy(m => m.GroupId)
                .ToList();

            foreach (var group in groupMessages)
            {
                var groupId = group.Key;
                string groupName = "Unnamed Group";
                ObservableCollection<User> participants = new ObservableCollection<User>();

                if (userGroups.TryGetValue(groupId, out var groupInfo))
                {
                    groupName = groupInfo.Name;
                    participants = new ObservableCollection<User>(groupInfo.Members);
                }

                chats.Add(new Chat
                {
                    Name = groupName,
                    GroupId = groupId,
                    Friend = null,
                    Participants = participants,
                    Messages = new ObservableCollection<Message>(group.ToList())
                });
            }

            // Step 7: Add empty chats for groups with no messages
            foreach (var group in userGroups.Values)
            {
                if (!chats.Any(c => c.GroupId == group.Id))
                {
                    chats.Add(new Chat
                    {
                        Name = group.Name,
                        GroupId = group.Id,
                        Friend = null,
                        Participants = new ObservableCollection<User>(group.Members),
                        Messages = new ObservableCollection<Message>()
                    });
                }
            }

            // Step 8: Add empty chats for users with no messages
            foreach (var user in allUsers.Values)
            {
                if (!conversationsByUser.ContainsKey(user.Id))
                {
                    chats.Add(new Chat
                    {
                        Name = user.UserName,
                        GroupId = null,
                        Friend = user,
                        Participants = null,
                        Messages = new ObservableCollection<Message>()
                    });
                }
            }

            return chats;
        }


    }
}
