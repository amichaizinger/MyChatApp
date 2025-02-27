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
        Task<bool> CreateGroupAsync(String name, List<Guid> members);
        Task<bool> AddUserToGroupAsync(Guid groupId, Guid userId);
        Task<bool> RemoveUserFromGroupAsync(Guid groupId, Guid userId);
        Task<List<Group>> GetUserGroupsAsync(Guid userId);
        Task<List<Models.User>> GetGroupMembersAsync(Guid GroupId);

    }
}
