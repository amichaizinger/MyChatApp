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

namespace ChatApp.Server.Services.Commands
{
    public class GetOnlineUsersCommand : ICommand
    {
        private readonly List<Guid> _online;


        public GetOnlineUsersCommand(List<Guid> online)
        {
            _online = online;
        }



        public async Task ExecuteAsync(Socket clientSocket)
        { 
            string content = " ";

            foreach (var on in _online)
            {
                content += on + " ";
            }
            Message message = new Message
            {
                Content = content,
                Command = CommandType.CreateGroup,
            };
            string json = JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await clientSocket.SendAsync(buffer, SocketFlags.None);
        }
    }
}
