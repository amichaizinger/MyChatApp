using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Commands.interfaces;

namespace ChatApp.Server.Commands
{
    public class NewUserCommand : ICommand
    {
        private readonly User _user;


        public NewUserCommand(User user)
        {
            _user = user;
        }



        public async Task ExecuteAsync(Socket clientSocket)
        {
              
            Message message = new Message
            {
                Content = JsonSerializer.Serialize(_user),
                Command = CommandType.CreateGroup,
                SenderId = _user.Id,
                SentAt = DateTime.Now,
            };
            string json = JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await clientSocket.SendAsync(buffer, SocketFlags.None);
        }
    }
}

