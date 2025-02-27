using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Server.Services.interfaces;

namespace ChatApp.Server.Services.Commands.interfaces
{
    public interface ICommand
    {
        Task ExecuteAsync(Socket clientSocket);
    }
}
