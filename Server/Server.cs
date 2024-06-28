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
using Server.ServerHandlers;


namespace Server
{
    public class Server
    {
        private readonly ICommunicationService _communicationService;
        private readonly IUserManagementService _userManagementService;
        private readonly IMessageService _messageService;
        private readonly CommandHandler _commandHandler;
        private readonly Dictionary<string, Action> commandDictionary = new Dictionary<string, Action>();

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
            _commandHandler = new CommandHandler(communicationService, userManagementService, messageService, ServerCreationDate, communicationOn, dataExchange);
            InitializeCommandDictionary();
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

                        if (commandDictionary.ContainsKey(request.Command.ToLower()))
                        {
                            commandDictionary[request.Command.ToLower()].Invoke();
                        }
                        else
                        {
                            _commandHandler.WrongCommand();
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

        private void InitializeCommandDictionary()
        {
            commandDictionary["help"] = _commandHandler.HelpCommand;
            commandDictionary["info"] = _commandHandler.InfoCommand;
            commandDictionary["uptime"] = _commandHandler.UpTimeCommand;
            commandDictionary["stop"] = _commandHandler.StopCommand;
            commandDictionary["register"] = _commandHandler.RegisterCommand;
            commandDictionary["login"] = _commandHandler.LoginCommand;
            commandDictionary["logout"] = _commandHandler.LogoutCommand;
            commandDictionary["users"] = _commandHandler.UsersCommand;
            commandDictionary["delete"] = _commandHandler.DeleteCommand;
            commandDictionary["message"] = _commandHandler.SendMessageCommand;
            commandDictionary["mailbox"] = _commandHandler.GetMessageCommand;
        }
    }
}