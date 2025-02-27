using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatAppSOLID.Services.Commands.interfaces;
using ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.NewFolder;
using ChatAppSOLID.Services.Interfaces;
using System.Net.Sockets;

namespace ChatAppSOLID.Services.Commands
{
    public class RegisterCommand : ICommand
    {
        private readonly string _username;
        private readonly string _password;
        private readonly Guid _senderId;

        public RegisterCommand(string username, string password, Guid senderId)
        {
            _username = username;
            _password = password;
            _senderId = senderId;
        }

        public async Task ExecuteAsync(Socket clientSocket)
        {
            var message = new Message
            {
                Command = CommandType.Register,
                SenderId = _senderId,
                Content = _username + " " + _password,
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
                ErrorOccurred?.Invoke(this, ex.Message);
            }
        }
    }
}
