using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Server.Data
{
    internal class Database
    {
        public class ChatDbContext : DbContext
        {
            public ChatDbContext()
            {
            }

            public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
            {
            }

            public DbSet<User> Users { get; set; }
            public DbSet<Message> Messages { get; set; }
            public DbSet<Group> Groups { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>()
                    .HasMany(u => u.Groups)
                    .WithMany(g => g.Members)
                    .UsingEntity(j => j.ToTable("GroupMembers"));

                modelBuilder.Entity<Message>()
                .HasOne<Group>() 
                .WithMany()     
                .HasForeignKey(m => m.GroupId);// GroupId as the foreign key

                modelBuilder.Entity<Message>().Ignore(m => m.Command);
            }
        }
    }
}
