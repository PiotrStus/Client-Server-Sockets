using Newtonsoft.Json;
using Shared.Classes;
using Shared.Interfaces;
using Shared.Requests;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ServerHandlers
{
    public class CommandHandler
    {
        private readonly ICommunicationService _communicationService;
        private readonly IUserManagementService _userManagementService;
        private readonly IMessageService _messageService;
        private bool _communicationOn;
        private bool _dataExchange;
        private DateTime _serverCreationDate;

        public CommandHandler(ICommunicationService communicationService, IUserManagementService userManagementService, IMessageService messageService,DateTime ServerCreationDate, bool communicationOn, bool dataExchange)
        {
            _communicationService = communicationService;
            _userManagementService = userManagementService;
            _messageService = messageService;
            _serverCreationDate = ServerCreationDate;
            _communicationOn = communicationOn;
            _dataExchange = dataExchange;

        }
        public void HelpCommand()
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

            if (_userManagementService.GetUser() != null)
            {
                helpMessage.Commands.Add("logout - user logout");
                helpMessage.Commands.Add("message - send a message");
                helpMessage.Commands.Add("mailbox - check your mailbox");
            }
            else
            {
                helpMessage.Commands.Add("login - login an user");
            }

            if (_userManagementService.IsAdmin())
            {
                helpMessage.Commands.Add("users - list of registered users");
                helpMessage.Commands.Add("delete - delete an user");
            }

            _communicationService.SendResponse(JsonConvert.SerializeObject(helpMessage));
        }
        public void InfoCommand()
        {
            var infoResponse = new InfoResponse
            {
                Message = "Server's informations",
                ServerCreated = _serverCreationDate,
                ServerVersion = Config.ServerVersion,
            };
            _communicationService.SendResponse(JsonConvert.SerializeObject(infoResponse));
        }
        public void RegisterCommand()
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
        public void UsersCommand()
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
        public void LoginCommand()
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
        public void LogoutCommand()
        {
            var response = _userManagementService.LogoutUser();
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = response }));
        }
        public void DeleteCommand()
        {
            var deleteRequest = new Request { Command = "Which user do you want to delete? " };
            _communicationService.SendResponse(JsonConvert.SerializeObject(deleteRequest));

            var data = _communicationService.ReceiveRequest();
            var userToDelete = JsonConvert.DeserializeObject<Request>(data);
            var deleteResponse = _userManagementService.DeleteUser(userToDelete.Command);

            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = deleteResponse }));
        }
        public void SendMessageCommand()
        {
            // request for indication of recipient
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = "Who do you want to send a message to?" }));

            // receiving the recipient
            var data = _communicationService.ReceiveRequest();
            var messageRecipient = JsonConvert.DeserializeObject<Request>(data);

            // request for a message
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = "Please enter your message" }));
            data = _communicationService.ReceiveRequest();
            var message = JsonConvert.DeserializeObject<Request>(data);

            // message validation
            var messageStatus = _messageService.SendMessage(messageRecipient.Command, message.Command);

            // sending a reply
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = messageStatus }));
        }
        public void GetMessageCommand()
        {
            var mails = _messageService.GetUserMessages();
            var mailsResponse = new MailsResponse
            {
                Message = "Mailbox: ",
                Mails = mails
            };
            _communicationService.SendResponse(JsonConvert.SerializeObject(mailsResponse));
        }
        public void WrongCommand()
        {
            _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = "Please enter a valid command! Type help for the command list." }));
        }
        public void UpTimeCommand()
        {
            var upTime = new UptimeResponse
            {
                Message = "Server's uptime [hh:mm:ss]",
                UpTime = (DateTime.Now.TimeOfDay - _serverCreationDate.TimeOfDay).ToString(@"hh\:mm\:ss")
            };
            _communicationService.SendResponse(JsonConvert.SerializeObject(upTime));
        }
        public void StopCommand()
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
                _communicationOn = false;
                _dataExchange = false;
            }
        }
    }
}
