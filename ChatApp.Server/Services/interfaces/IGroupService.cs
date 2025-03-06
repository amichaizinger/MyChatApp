using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Models;

namespace ChatApp.Server.Services.Interfaces
{
    public interface IGroupService
    {
        Task<bool> CreateGroupAsync(String name, List<string> members);
        Task<bool> NewGroupAsync(Group group);
        Task<bool> AddUserToGroupAsync(string groupId, string userId);
        Task<bool> RemoveUserFromGroupAsync(string groupId, string userId);
        Task<List<Group>> GetUserGroupsAsync(string userId);
        Task<List<Models.User>> GetGroupMembersAsync(string GroupId);

    }
}
