﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.interfaces;

namespace ChatApp.Server.Services.Commands.interfaces
{
    public interface ICommand
    {
        Task ExecuteAsync(Socket clientSocket);

        async Task SendMessageAsync(Socket clientSocket, Message message)
        {
            string json = JsonSerializer.Serialize(message);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            int length = jsonBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(length);

            await clientSocket.SendAsync(lengthBytes, SocketFlags.None);
            await clientSocket.SendAsync(jsonBytes, SocketFlags.None);
        }
    }
}
