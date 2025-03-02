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
        public bool IsConnected { get; set; } 
        public Socket ClientSocket { get; set; }
        private IPEndPoint _ipEndPoint;

        public event EventHandler<Message> MessageReceived;
        public event EventHandler<string> ErrorOccurred;
        public event EventHandler<bool> ConnectionStatusChanged;



        public async Task ConnectAsync(IPAddress ipAddress, int port)
        {
            {
                _ipEndPoint = new IPEndPoint(ipAddress, port);
                ClientSocket = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {

                        Debug.WriteLine("Socket created successfully.");
                        await ClientSocket.ConnectAsync(_ipEndPoint);
                        IsConnected = true;

                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, ex.Message);
                    IsConnected = false;
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket?.Close();
                ClientSocket?.Dispose();
                IsConnected = false;
                ConnectionStatusChanged?.Invoke(this, false);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex.Message);
            }
            finally
            {
                ClientSocket.Dispose();
            }
            ConnectionStatusChanged?.Invoke(this, false);
            IsConnected = false;

        }

    }

}
