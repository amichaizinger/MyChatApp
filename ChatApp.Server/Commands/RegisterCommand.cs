using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Services.Commands.interfaces;
using ChatApp.Server.Models;
using ChatApp.Server.Services.interfaces;
using System.Net.Sockets;
using System.Text.Json;
using ChatApp.Server.Commands;

namespace ChatApp.Server.Services.Commands
{
    public class RegisterCommand : ICommand
    {
        private readonly string _username;
        private readonly string _senderId;
        private readonly string _isRegistered;


        public RegisterCommand(string username, string senderId, string isRegistered)
        {
            _username = username;
            _senderId = senderId;
            _isRegistered = isRegistered;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            var message = new Message
            {
                Command = CommandType.Register,
                SenderId = _senderId,
                Content = _isRegistered + " " + _username,

            };
            await SendWithLength.SendMessageAsync(clientSocket, message);

        }
    }
}
