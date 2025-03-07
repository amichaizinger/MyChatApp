using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static ChatApp.Server.Data.Database;

namespace ChatAppSOLID.Services.NewFolder
{
    public class UserService : IUserService
    {
        private readonly ChatDbContext _db;

        public UserService()
        {
            _db = Program.ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ChatDbContext>();
        }


        public async Task<User> LoginAsync(string username, string password)
        {
            User user = await _db.Users.FirstOrDefaultAsync(user => user.UserName == username);
            if (user == null || user.Password != password)
            {
                return null;
            }

            await _db.SaveChangesAsync();
            return user;
        }


        public async Task<User> RegisterAsync(string username, string password)
        {
            if (await _db.Users.AnyAsync(user => user.UserName == username) || password.Length < 8) 
            {
                return null;
            }

            if (_db.Users.Count() > 20)  // TODO: to keep the app small remove once the debug is done
            {
                var users = await _db.Users.ToListAsync();
                _db.Users.RemoveRange(users);
                await _db.SaveChangesAsync();

                var messages = await _db.Messages.ToListAsync();
                _db.Messages.RemoveRange(messages);
                await _db.SaveChangesAsync();
            }

            User user = new User
            {
                UserName = username,
                Password = password,
                Id = Guid.NewGuid().ToString()
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateStatusAsync(string userId, bool status)
        {
            User user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            await _db.SaveChangesAsync();
            return true;

        }
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _db.Users.FindAsync(userId);
        }
    

    }
}
