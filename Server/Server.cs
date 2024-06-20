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
using Shared.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Server
{
    public class Server
    {
        private readonly ICommunicationService _communicationService;
        private readonly IUserManagementService _userManagementService;
        private readonly IMessageService _messageService;
        private static DateTime ServerCreationDate { get; set; }
        private static bool communicationOn = true;
        private static bool dataExchange = true;
        private static string helpMessage = "Choose one of the commands:\nuptime - server's lifetime\n" +
                                            "help - list of available commands\ninfo - server's version&creation date\n" +
                                            "register - register a new user\n" + "login - user login\n" +
                                            "stop - stops server and the client\n";
        public Server(ICommunicationService communicationService, IUserManagementService userManagementService, IMessageService messageService)
        {
            _communicationService = communicationService;
            _userManagementService = userManagementService;
            _messageService = messageService;
            ServerCreationDate = DateTime.Now;
        }
        public void Start()
        {
            try
            {
                while (communicationOn)
                {
                    _communicationService.SendResponse(JsonConvert.SerializeObject(helpMessage));
                    while (dataExchange)
                    {
                        string data = _communicationService.ReceiveRequest();
                        var request = JsonConvert.DeserializeObject<Request>(data);
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
                                    RegisterCommand();
                                    break;
                                }
                            case "login":
                                {
                                    LoginCommand();
                                    break;
                                }
                            case "logout":
                                {
                                    LogoutCommand();
                                    break;
                                }
                            case "users":
                                {
                                    UsersCommand();
                                    break;
                                }
                            case "delete":
                                {
                                    DeleteCommand();
                                    break;
                                }
                            case "message":
                                {
                                    MessageCommand();
                                    break;
                                }
                            default:
                                {
                                    WrongCommand();
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
        }
        private void UpTimeCommand()
        {
            var upTime = new UptimeResponse
            {
                Message = "Server's uptime [hh:mm:ss]",
                UpTime = (DateTime.Now.TimeOfDay - ServerCreationDate.TimeOfDay).ToString(@"hh\:mm\:ss")
            };
            _communicationService.SendResponse(JsonConvert.SerializeObject(upTime));
        }
        private void StopCommand()
        {
            try
            {
                var stop = new Request
                {
                    Command = "stop"
                };
                _communicationService.SendResponse(JsonConvert.SerializeObject(stop));
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
                ServerVersion = Config.ServerVersion,
            };
            _communicationService.SendResponse(JsonConvert.SerializeObject(infoResponse));
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
                    "stop - stops server and the client"
                }
            };

            if (_userManagementService.GetUser != null)
            {
                helpMessage.Commands.Add("logout - user logout");
                helpMessage.Commands.Add("message - send a message");
            }
            else
                helpMessage.Commands.Add("login - login an user");
            if (_userManagementService.IsAdmin())
            {
                helpMessage.Commands.Add("users - list of registered users");
                helpMessage.Commands.Add("delete - delete an user");
            }
            _communicationService.SendResponse(JsonConvert.SerializeObject(helpMessage));

        }
        private void RegisterCommand()
        {
            string loginRequest = "Please type your login:";
            string passwordRequest = "Please type your password:";

            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = loginRequest }));

            string data = _communicationService.ReceiveRequest();
            var loginData = JsonConvert.DeserializeObject<Request>(data);

            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = passwordRequest }));

            data = _communicationService.ReceiveRequest();
            var passwordData = JsonConvert.DeserializeObject<Request>(data);

            string registrationResult = _userManagementService.RegisterUser(loginData.Command, passwordData.Command);

            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = registrationResult }));
        }
        private void UsersCommand()
        {
            if (!_userManagementService.IsAdmin())
            {
                var response = new UsersResponse
                {
                    Message = "Access denied. Only admins can list users.",
                    Users = new List<string>()
                };
                _communicationService.SendResponse(JsonConvert.SerializeObject(response));
                return;
            }
            var users = _userManagementService.GetAllUsers();
            List<string> userNames = users.Select(user => user.Login).ToList();
            var userMessage = new UsersResponse
            {
                Message = "Available users",
                Users = userNames
            };
            _communicationService.SendResponse(JsonConvert.SerializeObject(userMessage));

        }
        private void LoginCommand()
        {
            string loginRequest = "Please type your login:";
            string passwordRequest = "Please type your password:";

            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = loginRequest }));

            string data = _communicationService.ReceiveRequest();
            var loginData = JsonConvert.DeserializeObject<Request>(data);

            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = passwordRequest }));


            data = _communicationService.ReceiveRequest();
            var passwordData = JsonConvert.DeserializeObject<Request>(data);

            var user = _userManagementService.LoginUser(loginData.Command, passwordData.Command);

            if (user != null)
            {
                _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = $"{loginData.Command} logged in" }));
            }
            else
                _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = "Wrong credantials!" }));
        }
        private void LogoutCommand()
        {
            var response = _userManagementService.LogoutUser();
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = response }));
        }
        private void DeleteCommand()
        {
            var deleteRequest = new Request { Command = "Which user do you want to delete? " };
            _communicationService.SendResponse(JsonConvert.SerializeObject(deleteRequest));

            var data = _communicationService.ReceiveRequest();
            var userToDelete = JsonConvert.DeserializeObject<Request>(data);
            var deleteResponse = _userManagementService.DeleteUser(userToDelete.Command);

            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = deleteResponse }));
        }
        private void MessageCommand()
        {
            // prosba o podanie adresata
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = "Who do you want to send a message to?" }));

            // otrzymanie adresata
            var data = _communicationService.ReceiveRequest();
            var messageRecipient = JsonConvert.DeserializeObject<Request>(data);
            Console.WriteLine(messageRecipient.Command);

            // prosba o podanie wiadomosci
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = "Please enter your message" }));

            // otrzymanie wiadomosci
            data = _communicationService.ReceiveRequest();
            var message = JsonConvert.DeserializeObject<Request>(data);
            Console.WriteLine(message.Command);

            // walidacja wiadomosci
            var messageStatus = _messageService.SendMessage(messageRecipient.Command, message.Command);
            //_messageService.Test();

            // wyslanie odpowiedzi
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = messageStatus }));
        }
        private void WrongCommand()
        {
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = "Please enter a valid command! Type help for the command list." }));
        }
    }
}