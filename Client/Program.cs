using Shared.Classes;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Shared.Responses;
using Shared.Requests;

namespace Client
{
    internal class Program
    {
        private static string? jsonMsg;
        private static bool exchangeOn = true;
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9013);
            Socket clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(endPoint);
                Console.WriteLine("######################################");
                Console.WriteLine("Conntected to the server.");
                Console.WriteLine("Socket connected to -> {0} ", clientSocket.RemoteEndPoint!.ToString());
                Console.WriteLine("######################################\n");
                byte[] messageReceived = new byte[1024];
                int byteReceived = clientSocket.Receive(messageReceived);
                Console.WriteLine("New message from Server: \n\n{0}",
                Encoding.ASCII.GetString(messageReceived,
                                           0, byteReceived));
                while (exchangeOn)
                {
                    Console.Write("\nEnter a new command: ");
                    string command = Console.ReadLine()!;
                    if (!string.IsNullOrEmpty(command))
                    {
                        Console.Clear();
                        var request = new Request { Command = command };
                        string jsonRequest = JsonConvert.SerializeObject(request);
                        byte[] messageSent = Encoding.ASCII.GetBytes(jsonRequest);
                        int byteSent = clientSocket.Send(messageSent);
                        byteReceived = clientSocket.Receive(messageReceived);
                        string newMessage = Encoding.ASCII.GetString(messageReceived, 0, byteReceived);
                        HandleResponse(newMessage, command, clientSocket);
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Please enter a valid command");
                    }
                }
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't establish connection to the server: " + ex.ToString());
            }
        }
        static void HandleResponse(string newMessage, string command, Socket clientSocket)
        {
            switch (command)
            {
                case "info":
                    var infoResponse = JsonConvert.DeserializeObject<InfoResponse>(newMessage);
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    Console.WriteLine("                 " + infoResponse.Message);
                    Console.WriteLine("\n######################################################\n");
                    Console.WriteLine($"Server's creation date&time: {infoResponse.ServerCreated}");
                    Console.WriteLine($"Server's version: {infoResponse.ServerVersion}");
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    break;
                case "register":
                    var loginResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.WriteLine(loginResponse.Command);
                    string userInput = Console.ReadLine()!;
                    var loginRequest = new Request { Command = userInput };
                    var jsonMsg = JsonConvert.SerializeObject(loginRequest);
                    clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
                    var messageReceived = new byte[1024];
                    int byteReceived = clientSocket.Receive(messageReceived);
                    newMessage = Encoding.ASCII.GetString(messageReceived, 0, byteReceived);
                    var passwordResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.Clear();
                    Console.WriteLine(passwordResponse.Command);
                    userInput = Console.ReadLine()!;
                    var passwordRequest = new Request { Command = userInput };
                    jsonMsg = JsonConvert.SerializeObject(passwordRequest);
                    clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
                    byteReceived = clientSocket.Receive(messageReceived);
                    newMessage = Encoding.ASCII.GetString(messageReceived, 0, byteReceived);
                    var userCreated = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.Clear();
                    Console.WriteLine(userCreated.Command);
                    break;
                case "login":
                    loginResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.WriteLine(loginResponse.Command);
                    userInput = Console.ReadLine()!;
                    loginRequest = new Request { Command = userInput };
                    jsonMsg = JsonConvert.SerializeObject(loginRequest);
                    clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
                    messageReceived = new byte[1024];
                    byteReceived = clientSocket.Receive(messageReceived);
                    newMessage = Encoding.ASCII.GetString(messageReceived, 0, byteReceived);
                    passwordResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.Clear();
                    Console.WriteLine(passwordResponse.Command);
                    userInput = Console.ReadLine()!;
                    passwordRequest = new Request { Command = userInput };
                    jsonMsg = JsonConvert.SerializeObject(passwordRequest);
                    clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
                    byteReceived = clientSocket.Receive(messageReceived);
                    newMessage = Encoding.ASCII.GetString(messageReceived, 0, byteReceived);
                    var loginStatus = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.Clear();
                    Console.WriteLine(loginStatus.Command);
                    break;
                case "logout":
                    var logoutResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    Console.WriteLine("             " + logoutResponse.Command);

                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    break;
                case "uptime":
                    var uptimeResponse = JsonConvert.DeserializeObject<UptimeResponse>(newMessage);
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    Console.WriteLine("              " + uptimeResponse.Message);
                    Console.WriteLine("\n######################################################\n");
                    Console.WriteLine($"                        {uptimeResponse.UpTime}");
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    break;
                case "help":
                    var helpResponse = JsonConvert.DeserializeObject<HelpResponse>(newMessage);
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    Console.WriteLine("                  " + helpResponse.Message);
                    Console.WriteLine("\n######################################################\n");
                    foreach (var availableCommand in helpResponse.Commands)
                    {
                    Console.WriteLine($"          {availableCommand}");
                    }
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    break;
                case "stop":
                    clientSocket.Close(); 
                    exchangeOn = false;
                    break;
                case "users":
                    var usersResponse = JsonConvert.DeserializeObject<UsersResponse>(newMessage);
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    Console.WriteLine("                  " + usersResponse.Message);
                    Console.WriteLine("\n######################################################\n");
                    foreach (var user in usersResponse.Users)
                    {
                        Console.WriteLine($"{user} ");
                    }
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    break;
                default:
                    Console.WriteLine("Unknown command. Type help for list of avaiable commands");
                    break;
            }
        }
    }
}
