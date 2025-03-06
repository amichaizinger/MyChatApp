using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Models;

namespace ChatApp.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> LoginAsync(string username, string password);
        Task<User> RegisterAsync(string username, string password);
        Task<bool> UpdateStatusAsync(string userId, bool status);
        Task<User> GetUserByIdAsync(string userId);

    }
}
