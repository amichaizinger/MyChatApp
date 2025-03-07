using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Commands;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Commands.interfaces;
using ChatApp.Server.Services.interfaces;

namespace ChatApp.Server.Services.Commands
{
    public class LoginCommand : ICommand
    {
        public string SenderId;
        public string IsCorrect;
        private readonly string _username;


        public LoginCommand(string username, string senderId, string isCorrect)
        {
            _username = username;
            SenderId = senderId;
            IsCorrect = isCorrect;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            Message message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                SenderId = SenderId,
                Content = IsCorrect + " " + _username,
                Command = CommandType.Login,
                SentAt = DateTime.Now
            };
            await SendWithLength.SendMessageAsync(clientSocket, message);

        }
    }
}
