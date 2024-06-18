using Shared.Classes;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Shared.Responses;
using Shared.Requests;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    internal class Program
    {

        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9013);
            Socket clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(endPoint);
            Console.WriteLine("######################################");
            Console.WriteLine("Conntected to the server.");
            Console.WriteLine("Socket connected to -> {0} ", clientSocket.RemoteEndPoint!.ToString());
            Console.WriteLine("######################################\n");
            var communicationService = new SocketCommunicationService(clientSocket);
            Client client = new Client(communicationService);
            client.Start();
        }
    }
}
