using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Shared.Classes
{
    public class SocketCommunicationService : ICommunicationService
    {
        private Socket _clientSocket;

        public SocketCommunicationService(Socket socket)
        {
            _clientSocket = socket;
        }
        public string ReceiveRequest()
        {
            byte[] buffer = new byte[1024];
            int numBytes = _clientSocket.Receive(buffer);
            return Encoding.ASCII.GetString(buffer, 0, numBytes);
        }

        public void SendResponse(string response)
        {
            _clientSocket.Send(Encoding.ASCII.GetBytes(response));
        }
    }
}
