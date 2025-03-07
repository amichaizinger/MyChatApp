using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Services.interfaces;
using ChatApp.Server.Models;
using System.Net.Sockets;
using ChatApp.Server.Services.Commands.interfaces;
using System.Text.Json;
using ChatApp.Server.Commands;

namespace ChatApp.Server.Services.Commands
{
    public class GetOnlineUsersCommand : ICommand
    {
        private readonly List<string> _online;


        public GetOnlineUsersCommand(List<string> onlineIds)
        {
            _online = onlineIds;
        }



        public async Task ExecuteAsync(Socket clientSocket)
        { 

            Message message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Content = JsonSerializer.Serialize(_online),
                Command = CommandType.GetOnlineUsers,
            };
            await SendWithLength.SendMessageAsync(clientSocket, message);

        }
    }
}
