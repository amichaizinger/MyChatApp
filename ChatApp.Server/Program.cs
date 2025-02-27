// See https://aka.ms/new-console-template for more information
using ChatApp.Server.Models;
using ChatApp.Server.Services.interfaces;
using ChatApp.Server.Services.Interfaces;
using ChatApp.Server.Services.NewFolder;
using ChatApp.Server.Services;
using ChatAppSOLID.Services.NewFolder;
using Microsoft.Extensions.DependencyInjection;
using static ChatApp.Server.Data.Database;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure dependency injection for services
        var services = new ServiceCollection();
        services.AddDbContext<ChatDbContext>(options => options.UseSqlite("Data Source=chatapp.db"));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IClientHandler, ClientHandler>();
        services.AddScoped<IChatServer, ChatServer>();

        var provider = services.BuildServiceProvider();

        // Create or ensure the database exists and seed initial data
        using (var scope = provider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
            db.Database.EnsureCreated(); // Create database if it doesn’t exist

        }

        // Start the chat server on port 8080
        var server = provider.GetRequiredService<IChatServer>();
        await server.StartAsync(8080);
    }
}