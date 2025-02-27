using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatAppSOLID.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppSOLID.data
{
    internal class Database
    {
        public class ChatDbContext : DbContext
        {
            public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
            {
            }

            public DbSet<User> Users { get; set; }
            public DbSet<Message> Messages { get; set; }
            public DbSet<Group> Groups { get; set; }
        }

    }
}

