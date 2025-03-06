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

namespace ChatAppSOLID.Services.Commands
{
    public class CreateGroupCommand : ICommand
    {
        private readonly Message _message;


        public CreateGroupCommand(Message message)
        {
            _message = message;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            await SendWithLength.SendMessageAsync(clientSocket, _message);

        }
    }
}
