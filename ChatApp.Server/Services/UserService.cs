using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static ChatApp.Server.Data.Database;

namespace ChatAppSOLID.Services.NewFolder
{
    internal class UserService : IUserService
    {
        private readonly ChatDbContext _db;

        public UserService()
        {
            _db = new ChatDbContext();
        }


        public async Task<User> LoginAsync(string username, string password)
        {
            User user = await _db.Users.FirstOrDefaultAsync(user => user.UserName == username);
            if (user == null || user.Password != password)
            {
                return null;
            }

            user.IsOnline = true;
            await _db.SaveChangesAsync();
            return user;
        }


        public async Task<User> RegisterAsync(string username, string password)
        {
            if (await _db.Users.AnyAsync(user => user.UserName == username) || password.Length < 8) // todo: check if password is 8 digits, q: mybe dowthis from main
            {
                return null;
            }

            User user = new User
            {
                UserName = username,
                Password = password,
                IsOnline = true
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateStatusAsync(Guid userId, bool status)
        {
            User user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.IsOnline = status;
            await _db.SaveChangesAsync();
            return true;

        }
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _db.Users.FindAsync(userId);
        }
    

    }
}
