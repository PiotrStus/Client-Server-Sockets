using Shared.Classes;
using Shared.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serverSocket = new Socket(Config.IpAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(Config.LocalEndPoint);
            serverSocket.Listen(1);
            while (true)
            {
                Console.WriteLine("Server is waiting for connection on localhost:9013...");
                var clientSocket = serverSocket.Accept();
                Console.WriteLine("Client connected.");
                var communicationService = new SocketCommunicationService(clientSocket);
                var usersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserFiles/users.json");
                var messagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserFiles/messages.json");


                IUserManagementService userManagementService = new UserManagementService(usersPath);
                IMessageService messageService = new MessageService(userManagementService, messagesPath);

                Server server = new Server(communicationService, userManagementService, messageService);
                server.Start();
            }
        }
    }
}
