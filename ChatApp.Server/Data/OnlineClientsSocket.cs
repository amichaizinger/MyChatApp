using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Server.Data
{
    public static class OnlineClientsSocket
    {
        public static Dictionary<string, Socket> OnlineClients = new Dictionary<string, Socket>();
    }
}
