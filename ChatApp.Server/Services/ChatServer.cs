using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Services.interfaces;

namespace ChatApp.Server.Services
{
    public class ChatServer : IChatServer
    {
        private IPEndPoint _endPoint;
        private IPAddress _ipAddress = IPAddress.Any;
        private Socket _clientSocket;
        private readonly IClientHandler _clientHandler;


        public ChatServer(IClientHandler clientHandler)
        {
            _clientHandler = clientHandler;
        }

        public async Task StartAsync(int port)
        {
            _endPoint = new IPEndPoint(_ipAddress, port);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(_endPoint);
            server.Listen(100);

            while (true)
            {
                _clientSocket = await server.AcceptAsync();

                Task.Run(() => _clientHandler.HandleClientAsync(_clientSocket));
            }
        }

        public void StopAsync(Socket clientSocket)
        {
                clientSocket.Shutdown(SocketShutdown.Both); // todo: use in clientHAndler
        }

       
    }
}
