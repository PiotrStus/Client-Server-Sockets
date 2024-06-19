using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class ConnectionInfoPresenter
    {
        public static void ShowConnectionDetails(Socket clientSocket)
        {
            Console.WriteLine("######################################");
            Console.WriteLine("Conntected to the server.");
            Console.WriteLine("Socket connected to -> {0} ", clientSocket.RemoteEndPoint!.ToString());
            Console.WriteLine("######################################\n");
        }
    }
}
