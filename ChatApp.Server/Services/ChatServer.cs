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
        public Socket clientSocket;
        public static Dictionary<string, Socket> clients = new Dictionary<string, Socket>();



        public async Task StartAsync(int port)
        {
            _endPoint = new IPEndPoint(_ipAddress, port);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(_endPoint);
            server.Listen(100);

            while (true)
            {
                clientSocket = await server.AcceptAsync();
                Console.WriteLine("Client connected");

                ClientHandler _clientHandler = new ClientHandler();
                Task.Run(() => _clientHandler.HandleClientAsync(clientSocket));

            }
        }

        public void StopAsync(Socket clientSocket)
        {
                clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            clientSocket.Dispose();
        }

       
    }
}
