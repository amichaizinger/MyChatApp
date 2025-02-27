using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChatApp.Server.Models;
using static ChatApp.Server.Data.Database;

namespace ChatApp.Server.Services.NewFolder
{
    public class GroupService : IGroupService
    {
        private readonly ChatDbContext _db;

        public GroupService()
        {
            _db = new ChatDbContext();
        }
        public async Task<bool> AddUserToGroupAsync(Guid groupId, Guid userId)
        {
            _db.Groups.FirstOrDefault(g => g.Id == groupId).Members.Add(_db.Users.FirstOrDefault(u => u.Id == userId));
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveUserFromGroupAsync(Guid groupId, Guid userId)
        {
            _db.Groups.FirstOrDefault(g => g.Id == groupId).Members.Remove(_db.Users.FirstOrDefault(u => u.Id == userId));
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CreateGroupAsync(String name, List<Guid>members)
        {
            _db.Groups.Add(new Group{ Name = name, Members = _db.Users.Where(u => members.Contains(u.Id)).ToList() });
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<List<Models.Group>> GetUserGroupsAsync(Guid userId)
        {
            return await _db.Groups.Where(g => g.Members.Any(m => m.Id == userId)).ToListAsync();
        }

        public async Task<List<Models.User>> GetGroupMembersAsync(Guid GroupId)
        {
            return await _db.Groups.Where(g => g.Id == GroupId).SelectMany(g => g.Members).ToListAsync();
        }

    }
    
}
