using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Commands.interfaces;
using ChatApp.Server.Services.interfaces;


namespace ChatApp.Server.Services.Commands
{
    public class AddUserToGroupCommand : ICommand
    {
        private readonly Message _message;



        public AddUserToGroupCommand(Message message)
        {
            _message = message;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            string json = JsonSerializer.Serialize(_message);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await clientSocket.SendAsync(buffer, SocketFlags.None);
        }
    }
}