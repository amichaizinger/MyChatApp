using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChatApp.Server.Models;
using static ChatApp.Server.Data.Database;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Server.Services.NewFolder
{
    public class GroupService : IGroupService
    {
        private readonly ChatDbContext _db;

        public GroupService()
        {
            _db = Program.ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ChatDbContext>();
        }
        public async Task<bool> AddUserToGroupAsync(string groupId, string userId)
        {
            _db.Groups.FirstOrDefault(g => g.Id == groupId).Members.Add(_db.Users.FirstOrDefault(u => u.Id == userId));
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveUserFromGroupAsync(string groupId, string userId)
        {
            _db.Groups.FirstOrDefault(g => g.Id == groupId).Members.Remove(_db.Users.FirstOrDefault(u => u.Id == userId));
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CreateGroupAsync(String name, List<string> members)
        {
            _db.Groups.Add(new Group{ Name = name, Members = _db.Users.Where(u => members.Contains(u.Id)).ToList() });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> NewGroupAsync(Group group)
        {
            List<string> userIds = group.Members.Select(u => u.Id).ToList();

            // 2. Fetch existing users from the database by their IDs
            var groupsUsers = _db.Users
                .Where(u => userIds.Contains(u.Id))
                .ToList();

            group.Members.Clear();
            group.Members = groupsUsers;

            _db.Groups.Add(group);
            await _db.SaveChangesAsync();
            return true;
        }
 
        public async Task<List<Models.Group>> GetUserGroupsAsync(string userId)
        {
            return await _db.Groups.Where(g => g.Members.Any(m => m.Id == userId)).ToListAsync();
        }

        public async Task<List<Models.User>> GetGroupMembersAsync(string GroupId)
        {
            return await _db.Groups.Where(g => g.Id == GroupId).SelectMany(g => g.Members).ToListAsync();
        }

    }
    
}
