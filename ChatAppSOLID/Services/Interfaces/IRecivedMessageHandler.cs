using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppSOLID.Services.Interfaces
{
    public interface IRecivedMessageHandler
    {
        Task RecivedCommandHandlerAsync(Socket clientSocket);

    }
}
