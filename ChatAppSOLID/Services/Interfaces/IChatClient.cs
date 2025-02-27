using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatAppSOLID.Models;

namespace ChatAppSOLID.Services.Interfaces
{
    public interface IChatClient
    {
        bool IsConnected { get;  set; }
        Socket ClientSocket { get; set; }
        Task ConnectAsync(IPAddress ipAddress, int port, Guid userId);
        void Disconnect();

    }
}
