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
using System.Diagnostics;

namespace ChatApp.Server.Services.Commands
{
    public class SendMessageCommand : ICommand
    {
        private readonly Message _message;
        public SendMessageCommand(Message message)
        {
            _message = message;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            try
            {
                string json = JsonSerializer.Serialize(_message);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                await clientSocket.SendAsync(buffer, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
