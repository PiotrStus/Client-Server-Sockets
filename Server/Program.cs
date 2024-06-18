using Shared.Classes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPAddress IpAddress = IPAddress.Parse("127.0.0.1");
            int Port = 9013;
            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IpAddress, Port));
            serverSocket.Listen(1);

            while (true)
            {
                Console.WriteLine("Server is waiting for connection on localhost:9013...");
                var clientSocket = serverSocket.Accept();
                Console.WriteLine("Client connected.");

                var communicationService = new SocketCommunicationService(clientSocket);
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserFiles/users.json");
                Server server = new Server(communicationService, new UserManagementService(path));
                server.Start();
            }
        }
    }
}
