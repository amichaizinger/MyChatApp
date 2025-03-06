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
    public static IServiceProvider ServiceProvider { get; private set; }
    static async Task Main(string[] args)
    {
        // Configure dependency injection for ChatDbContext before anything else
        var services = new ServiceCollection();
        services.AddDbContext<ChatDbContext>(options => options.UseSqlite("Data Source=chatapp.db"));

        ServiceProvider = services.BuildServiceProvider();

        // Create or ensure the database exists
        try
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
                db.Database.EnsureCreated();
                Console.WriteLine("Database created successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create database: {ex.Message}");
            return; // Exit if database creation fails
        }


        // Start the chat server on port 8080
        ChatServer chatServer = new ChatServer();
        await chatServer.StartAsync(8080);
    }
}