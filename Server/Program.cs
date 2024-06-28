using Shared.Classes;
using Shared.Interfaces;
using Shared.Classes.Services;
using Shared.Classes.Repositories.FileType;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.Classes;

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
                var userRepositoryFile = new UserFileRepository(usersPath);
                var messageRepositoryFile = new MessageFileRepository(messagesPath);


                IUserManagementService userManagementService = new UserManagementService(userRepositoryFile);

                IMessageService messageService = new MessageService(userManagementService, messageRepositoryFile);

                Server server = new Server(communicationService, userManagementService, messageService);
                server.Start();
            }
        }
    }
}
