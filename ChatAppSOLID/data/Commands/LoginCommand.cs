using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.Commands.interfaces;
using ChatAppSOLID.Services.Interfaces;

namespace ChatAppSOLID.Services.Commands
{
    public class LoginCommand : ICommand
    {
        public string SenderId { get; }

        private readonly string _username;
        private readonly string _password;

        public LoginCommand(string username, string password, string senderId)
        {
            _username = username;
            _password = password;
            SenderId = senderId;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            Message message = new Message
            {
                SenderId = SenderId,
                Content = _username + " " + _password,
                Command = CommandType.Login,
                SentAt = DateTime.Now
            };
            try
            {
                // Convert Message object to JSON string
                string jsonMessage = JsonSerializer.Serialize(message);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
                await clientSocket.SendAsync(buffer, SocketFlags.None);

            }

            catch (Exception ex)
            {
                Debug.WriteLine("message not sent");
            }
        }
    }
}
