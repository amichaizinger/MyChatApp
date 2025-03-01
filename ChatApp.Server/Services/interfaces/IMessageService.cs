using ChatApp.Server.Models;

namespace ChatApp.Server.Services.Interfaces
{
    public interface IMessageService
    {
        Task<bool> SaveMessageAsync(Message message);
        Task<List<Chat>> GetChatHistoryAsync(Guid userId);

    }
}