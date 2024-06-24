using Newtonsoft.Json;
using Shared.Interfaces;
using Shared.Requests;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        private static bool _exchangeOn = true;
        private readonly ICommunicationService _communicationService;
        public Client(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }
        public void Start()
        {
            try
            {
                string data = _communicationService.ReceiveRequest();
                var request1 = JsonConvert.DeserializeObject<string>(data);

                Console.WriteLine("New message from Server: \n\n{0}", request1);
                while (_exchangeOn)
                {
                    Console.Write("\nEnter a new command: ");
                    string command = Console.ReadLine()!;
                    if (!string.IsNullOrEmpty(command))
                    {
                        Console.Clear();
                        _communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = command }));
                        string newMessage = _communicationService.ReceiveRequest();
                        HandleResponse(newMessage, command, _communicationService);
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Please enter a valid command");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't establish connection to the server: " + ex.ToString());
            }
        }

        private void HandleInfo(string newMessage)
        {
            var infoResponse = JsonConvert.DeserializeObject<InfoResponse>(newMessage);
            CommunicationMessages.ShowInfo(infoResponse.Message, infoResponse.ServerCreated, infoResponse.ServerVersion);
        }

        private string GetUserInput()
        {
            return Console.ReadLine()!;
        }

        private string CreateRequest(string command)
        {
            return JsonConvert.SerializeObject(new Request { Command = command });
        }

        private T ReceiveAndDeserialize<T>()
        {
            string messageReceived = _communicationService.ReceiveRequest();
            return JsonConvert.DeserializeObject<T>(messageReceived);
        }

        private void HandleRegister(string newMessage)
        {
            var loginResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLine(loginResponse.Command);

            string userInput = GetUserInput();
            _communicationService.SendResponse(CreateRequest(userInput));

            var passwordResponse = ReceiveAndDeserialize<Request>();
            CommunicationMessages.ShowLine(passwordResponse.Command);

            userInput = GetUserInput();
            _communicationService.SendResponse(CreateRequest(userInput));

            var userCreated = ReceiveAndDeserialize<Request>();
            CommunicationMessages.ShowLine(userCreated.Command);

        }

        private void HandleLogin(string newMessage)
        {
            var loginResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLine(loginResponse.Command);

            string userInput = GetUserInput();
            _communicationService.SendResponse(CreateRequest(userInput));

            var passwordResponse = ReceiveAndDeserialize<Request>();
            CommunicationMessages.ShowLine(passwordResponse.Command);

            userInput = GetUserInput();
            _communicationService.SendResponse(CreateRequest(userInput));

            var loginStatus = ReceiveAndDeserialize<Request>();
            CommunicationMessages.ShowLine(loginStatus.Command);
        }

        private void HandleLogout(string newMessage)
        {
            var logoutResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLogout(logoutResponse.Command);
        }

        private void HandleDelete(string newMessage)
        {
            var deleteResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLine(deleteResponse.Command);

            string userInput = GetUserInput();
            _communicationService.SendResponse(CreateRequest(userInput));

            deleteResponse = ReceiveAndDeserialize<Request>();
            CommunicationMessages.ShowLine(deleteResponse.Command);
        }

        private void HandleUptime(string newMessage)
        {
            var uptimeResponse = JsonConvert.DeserializeObject<UptimeResponse>(newMessage);
            CommunicationMessages.ShowUptime(uptimeResponse.Message, uptimeResponse.UpTime);
        }

        private void HandleHelp(string newMessage)
        {
            var helpResponse = JsonConvert.DeserializeObject<HelpResponse>(newMessage);
            CommunicationMessages.ShowHelp(helpResponse.Message, helpResponse.Commands);
        }

        private void HandleMessage(string newMessage)
        {
            var messageResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLine(messageResponse.Command);

            string userInput = GetUserInput();
            _communicationService.SendResponse(CreateRequest(userInput));

            messageResponse = ReceiveAndDeserialize<Request>();
            CommunicationMessages.ShowLine(messageResponse.Command);

            userInput = GetUserInput();
            _communicationService.SendResponse(CreateRequest(userInput));

            messageResponse = ReceiveAndDeserialize<Request>();
            CommunicationMessages.ShowLine(messageResponse.Command);
        }

        private void HandleMailbox(string newMessage)
        {
            var mailboxResponse = JsonConvert.DeserializeObject<MailsResponse>(newMessage);
            CommunicationMessages.ShowMailbox(mailboxResponse.Message, mailboxResponse.Mails);
        }

        private void HandleUsers(string newMessage)
        {
            var usersResponse = JsonConvert.DeserializeObject<UsersResponse>(newMessage);
            CommunicationMessages.ShowUsers(usersResponse.Message, usersResponse.Users);
        }

        private void HandleDefault(string newMessage)
        {
            var wrongResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLine(wrongResponse.Command);
        }

        public void HandleResponse(string newMessage, string command, ICommunicationService communicationService)
        {
            switch (command)
            {
                case "info":
                    HandleInfo(newMessage);
                    break;
                case "register":
                    HandleRegister(newMessage);
                    break;
                case "login":
                    HandleLogin(newMessage);
                    break;
                case "logout":
                    HandleLogout(newMessage);
                    break;
                case "delete":
                    HandleDelete(newMessage);
                    break;
                case "uptime":
                    HandleUptime(newMessage);
                    break;
                case "help":
                    HandleHelp(newMessage);
                    break;
                case "message":
                    HandleMessage(newMessage);
                    break;
                case "mailbox":
                    HandleMailbox(newMessage);
                    break;
                case "stop":
                    _exchangeOn = false;
                    break;
                case "users":
                    HandleUsers(newMessage);
                    break;
                default:
                    HandleDefault(newMessage);
                    break;
            }
        }
    }
}
