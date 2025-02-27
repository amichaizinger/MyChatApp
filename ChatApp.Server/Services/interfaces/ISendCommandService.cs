using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Models;

namespace ChatApp.Server.Services.interfaces
{
    public interface ISendCommandService
    {
        Task SendCommandAsync(Message message, Socket clientSocket);
    }
}
