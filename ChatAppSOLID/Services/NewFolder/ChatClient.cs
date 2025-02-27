using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static ChatAppSOLID.data.Database;
using ChatAppSOLID.Services.Interfaces;
using System.Text.Json;
using ChatAppSOLID.Models;

namespace ChatAppSOLID.Services.NewFolder
{
    public class ChatClient : IChatClient
    {
        public bool IsConnected { get; set; } = false;
        public Socket ClientSocket { get; set; }
        private IPEndPoint _ipEndPoint;
        private readonly IRecivedMessageHandler _recivedMessageHandler = new RecivedMessageHandler();

        public event EventHandler<Message> MessageReceived;
        public event EventHandler<string> ErrorOccurred;
        public event EventHandler<bool> ConnectionStatusChanged;



        public async Task ConnectAsync(IPAddress ipAddress, int port, Guid userId)
        {
            {
                _ipEndPoint = new IPEndPoint(ipAddress, port);

                try
                {
                    ClientSocket = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Debug.WriteLine("Socket created successfully.");
                    await ClientSocket.ConnectAsync(_ipEndPoint);
                    IsConnected = true;

                    await Task.Run(() => _recivedMessageHandler.RecivedCommandHandlerAsync(ClientSocket));
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, ex.Message);
                }
            }
        }

        public void Disconnect()
        {
            ClientSocket.Shutdown(SocketShutdown.Both);
            ConnectionStatusChanged?.Invoke(this, false);
            IsConnected = false;

        }

    }

}
