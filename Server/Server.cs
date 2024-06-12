using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private static IPAddress IpAddress = IPAddress.Parse("127.0.0.1");
        private const int Port = 9013;
        private static string ServerVersion { get; set; } = "0.1.0";
        private static DateTime ServerCreationDate { get; set; }
        private static string? jsonMsg;
        private Socket clientSocket;
        private IPEndPoint endpoint;
        private Socket serverSocket;
        private static bool communicationOn = true;
        private static bool dataExchange = true;
        private static string helpMessage = "Choose one of the commands:\nuptime - server's lifetime\n" +
                                            "help - list of available commands\ninfo - server's version&creation date\n" +
                                            "register - register a new user\n" + "stop - stops server and the client\n";
        private List<User> users = new List<User>();
        public Server()
        {
            endpoint = new IPEndPoint(IpAddress, Port);
            serverSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ServerCreationDate = DateTime.Now;
        }
        public void Start()
        {
            try
            {
                serverSocket.Bind(endpoint);

                serverSocket.Listen(1);
                while (communicationOn)
                {
                    Console.WriteLine("Server is waiting for connection on localhost:9013...");
                    clientSocket = serverSocket.Accept();
                    Console.WriteLine("Client connected.");
                    var messageSent = Encoding.ASCII.GetBytes(helpMessage);
                    int bytesSent = clientSocket.Send(messageSent);
                    var buffer = new byte[1024];



                    while (dataExchange)
                    {
                        string data = null!;
                        int numByte = clientSocket.Receive(buffer);
                        data += Encoding.ASCII.GetString(buffer, 0, numByte);
                        Console.WriteLine("Command received -> {0}", data); ;
                        switch (data.ToLower())
                        {
                            case "help":
                                {
                                    HelpCommand();
                                    break;
                                }
                            case "info":
                                {
                                    InfoCommand();
                                    break;
                                }
                            case "uptime":
                                {
                                    UpTimeCommand();
                                    break;
                                }
                            case "stop":
                                {
                                    StopCommand();
                                    break;
                                }
                            case "register":
                                {
                                    RegisterCommand();
                                    break;
                                }
                            default:
                                {
                                    jsonMsg = JsonSerializer.Serialize("Please enter a valid command!\n\n" + helpMessage);
                                    clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                serverSocket.Close();
            }
        }
        private void UpTimeCommand()
        {
            jsonMsg = JsonSerializer.Serialize($"Server's lifetime [hh:mm:ss]: {(DateTime.Now.TimeOfDay - ServerCreationDate.TimeOfDay).ToString(@"hh\:mm\:ss")}");
            clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
        }
        private void StopCommand()
        {
            try
            {
                jsonMsg = JsonSerializer.Serialize("stop");
                clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                communicationOn = false;
                dataExchange = false;
            }
        }
        private void InfoCommand()
        {
            jsonMsg = JsonSerializer.Serialize($"Server's creation date&time: {ServerCreationDate}, server version: {ServerVersion}");
            clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
        }
        private void HelpCommand()
        {
            jsonMsg = JsonSerializer.Serialize(helpMessage);
            clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
        }
        private void RegisterCommand()
        {
            jsonMsg = JsonSerializer.Serialize("register");
            clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
            string loginData = null!;
            var loginBuffer = new byte[1024];
            int loginBytesReceived = clientSocket.Receive(loginBuffer);
            jsonMsg = Encoding.ASCII.GetString(loginBuffer, 0, loginBytesReceived);
            loginData = JsonSerializer.Deserialize<string>(jsonMsg)!;
            Console.WriteLine("login: " + loginData);
            string passwordData = null!;
            var passwordBuffer = new byte[1024];
            int passwordBytesReceived = clientSocket.Receive(passwordBuffer);
            jsonMsg = Encoding.ASCII.GetString(passwordBuffer, 0, passwordBytesReceived);
            passwordData = JsonSerializer.Deserialize<string>(jsonMsg)!;
            Console.WriteLine("password: " + passwordData);

            Console.WriteLine($"User: {loginData}, password: {passwordData} created.");

            users.Add(new RegularUser(loginData, passwordData));
            string jsonString = JsonSerializer.Serialize(users);
            Console.WriteLine(jsonString);
            var FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserFiles/users.json");

            using (StreamWriter writer = new StreamWriter(FilesDirectory))
            {
                writer.WriteLine(jsonString);
            }
        }
    }
}
