using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.interfaces;

namespace ChatApp.Server.Services
{
    public class SendCommandService : ISendCommandService
    {
        public async Task SendCommandAsync(Message message, Socket clientSocket)
        {
            try
            {
                // Convert Message object to JSON string
                string jsonMessage = JsonSerializer.Serialize(message);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
                await clientSocket.SendAsync(buffer, SocketFlags.None);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
