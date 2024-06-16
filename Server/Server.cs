using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Requests;
using Shared.Responses;
using Shared.Classes.Shared.Classes;


namespace Server
{
    public class Server
    {
        private string filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserFiles/users.json");
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
                                            "register - register a new user\n" + "users - list of registered users\n" + "stop - stops server and the client\n";

        AdminUser adminUser = new AdminUser("admin", "admin123");
        User currentUser;
        private List<User> users = new List<User>();
        public Server()
        {
            endpoint = new IPEndPoint(IpAddress, Port);
            serverSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ServerCreationDate = DateTime.Now;
            AddAdmin(adminUser);
        }
        private void AddAdmin(AdminUser adminUser)
        {
            using (StreamWriter writer = new StreamWriter(filesDirectory))
            {
                users.Add(adminUser);
                var admin = JsonConvert.SerializeObject(adminUser);
                writer.WriteLine(admin);
            }
        }

        public async Task Start()
        {
            try
            {
                serverSocket.Bind(endpoint);

                serverSocket.Listen(1);
                while (communicationOn)
                {
                    Console.WriteLine("Server is waiting for connection on localhost:9013...");
                    clientSocket = await serverSocket.AcceptAsync();
                    Console.WriteLine("Client connected.");
                    var messageSent = Encoding.ASCII.GetBytes(helpMessage);
                    int bytesSent = await clientSocket.SendAsync(messageSent);
                    var buffer = new byte[1024];



                    while (dataExchange)
                    {
                        string data = null!;
                        int numByte = clientSocket.Receive(buffer);
                        data += Encoding.ASCII.GetString(buffer, 0, numByte);
                        var request = JsonConvert.DeserializeObject<Request>(data);
                        
                        Console.WriteLine("Command received -> {0}", data);
                        Console.WriteLine(request.Command.ToLower());
                        //Console.ReadKey();
                        switch (request.Command.ToLower())
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
                                    await RegisterCommand();
                                    break;
                                }
                            case "users":
                                {
                                    await UsersCommand();
                                    break;
                                }
                            default:
                                {
                                    jsonMsg = JsonConvert.SerializeObject("Please enter a valid command!\n\n" + helpMessage);
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
            var upTime = new UptimeResponse
            {
                Message = "Server's uptime [hh:mm:ss]",
                UpTime = (DateTime.Now.TimeOfDay - ServerCreationDate.TimeOfDay).ToString(@"hh\:mm\:ss")
            };
            jsonMsg = JsonConvert.SerializeObject(upTime);
            Console.WriteLine(jsonMsg);
            clientSocket.SendAsync(Encoding.ASCII.GetBytes(jsonMsg));
        }
        private void StopCommand()
        {
            try
            {
                var stop = new Request
                {
                    Command = "stop"
                };
                jsonMsg = JsonConvert.SerializeObject(stop);
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
            var infoResponse = new InfoResponse
            {
                Message = "Server's informations",
                ServerCreated = ServerCreationDate,
                ServerVersion = ServerVersion,
            };
            jsonMsg = JsonConvert.SerializeObject(infoResponse);
            clientSocket.SendAsync(Encoding.ASCII.GetBytes(jsonMsg));
        }
        private void HelpCommand()
        {
            var helpMessage = new HelpResponse
            {
                Message = "Available commands:",
                Commands = new List<string>
                {
                    "help - list of available commands",
                    "info - server's version&creation date",
                    "uptime - server's lifetime",
                    "register - register a new user",
                    "users - list of registered users",
                    "stop - stops server and the client",
                }
            };

            jsonMsg = JsonConvert.SerializeObject(helpMessage);
            clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
        }
        private async Task RegisterCommand()
        {
            string loginRequest = "Please type your login:";
            string passwordRequest = "Please type your password:";
            var buffer = new byte[1024];

            var request = new Request { Command = loginRequest };
            jsonMsg = JsonConvert.SerializeObject(request);
            await clientSocket.SendAsync(Encoding.ASCII.GetBytes(jsonMsg));
            
            int loginBytesReceived = await clientSocket.ReceiveAsync(buffer);
            jsonMsg = Encoding.ASCII.GetString(buffer, 0, loginBytesReceived);
            var loginData = JsonConvert.DeserializeObject<Request>(jsonMsg)!;
            Console.WriteLine("login: " + loginData.Command);

            request = new Request { Command = passwordRequest };
            jsonMsg = JsonConvert.SerializeObject(request);
            await clientSocket.SendAsync(Encoding.ASCII.GetBytes(jsonMsg));

            int passwordBytesReceived = await clientSocket.ReceiveAsync(buffer);
            jsonMsg = Encoding.ASCII.GetString(buffer, 0, passwordBytesReceived);
            var passwordData = JsonConvert.DeserializeObject<Request>(jsonMsg)!;
            Console.WriteLine("password: " + passwordData.Command);
            users.Add(new RegularUser(loginData.Command, passwordData.Command));
            string jsonUsers = JsonConvert.SerializeObject(users);
            Console.WriteLine("jsonString: " + jsonUsers);
            using (StreamWriter writer = new StreamWriter(filesDirectory))
            {
                writer.WriteLine(jsonUsers);
            }
            string userCreated = $"User: {loginData.Command} created.";
            request = new Request { Command = userCreated };
            jsonMsg = JsonConvert.SerializeObject(request);
            await clientSocket.SendAsync(Encoding.ASCII.GetBytes(jsonMsg));
        }

        private async Task UsersCommand()
        {
            using (StreamReader reader = new StreamReader(filesDirectory)) 
            {
                var jsonUsers = await reader.ReadToEndAsync();
                Console.WriteLine(jsonUsers);
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new UserConverter() },
                };

                try
                { var users = JsonConvert.DeserializeObject<List<User>>(jsonUsers, settings); 
                }
                catch (JsonSerializationException)
                {
                    var user = JsonConvert.DeserializeObject<User>(jsonUsers, settings);
                    users = new List<User> { user };
                }
                List<string> userNames = new List<string>();
                foreach (var user in users )
                {
                    userNames.Add(user.Login);
                }
                
                var userMessage = new UsersResponse
                {
                    Message = "Available users",
                    Users = userNames
                };
                jsonMsg = JsonConvert.SerializeObject(userMessage);
                Console.Write(jsonMsg);
                //Console.ReadKey(); 
                clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
            }
        }
    }
}
