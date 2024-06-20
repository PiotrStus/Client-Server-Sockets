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
        private static bool exchangeOn = true;
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
                while (exchangeOn)
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
        static void HandleResponse(string newMessage, string command, ICommunicationService communicationService)
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
                    communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = userInput }));
                    string messageReceived = communicationService.ReceiveRequest();
                    var passwordResponse = JsonConvert.DeserializeObject<Request>(messageReceived);
                    Console.Clear();
                    Console.WriteLine(passwordResponse.Command);
                    userInput = Console.ReadLine()!;
                    communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = userInput }));
                    messageReceived = communicationService.ReceiveRequest();
                    var userCreated = JsonConvert.DeserializeObject<Request>(messageReceived);
                    Console.Clear();
                    Console.WriteLine(userCreated.Command);
                    break;
                case "login":
                    loginResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.WriteLine(loginResponse.Command);
                    userInput = Console.ReadLine()!;
                    communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = userInput }));
                    messageReceived = communicationService.ReceiveRequest();
                    passwordResponse = JsonConvert.DeserializeObject<Request>(messageReceived);
                    Console.Clear();
                    Console.WriteLine(passwordResponse.Command);
                    userInput = Console.ReadLine()!;
                    communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = userInput }));
                    messageReceived = communicationService.ReceiveRequest();
                    var loginStatus = JsonConvert.DeserializeObject<Request>(messageReceived);
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
                case "delete":
                    //fetching data from server
                    var deleteResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.WriteLine(deleteResponse.Command);
                    // user's input data and send to server
                    userInput = Console.ReadLine()!;
                    communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = userInput }));
                    // accept/reject response from server
                    messageReceived = communicationService.ReceiveRequest();
                    deleteResponse = JsonConvert.DeserializeObject<Request>(messageReceived);
                    Console.Clear();
                    Console.WriteLine(deleteResponse.Command);
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
                    Console.WriteLine("                  " + helpResponse?.Message);
                    Console.WriteLine("\n######################################################\n");
                    foreach (var availableCommand in helpResponse?.Commands)
                    {
                        Console.WriteLine($"          {availableCommand}");
                    }
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    break;
                case "message":
                    // request to provide the recipient
                    var messageResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.WriteLine(messageResponse.Command);

                    // sending the recipient
                    userInput = Console.ReadLine()!;
                    communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = userInput }));

                    // prosba o podanie wiadomosci
                    messageReceived = communicationService.ReceiveRequest();
                    messageResponse = JsonConvert.DeserializeObject<Request>(messageReceived);
                    Console.WriteLine(messageResponse.Command);

                    // request for a message
                    userInput = Console.ReadLine()!;
                    communicationService.SendResponse(JsonConvert.SerializeObject(new Request { Command = userInput }));

                    // information about the message status
                    messageReceived = communicationService.ReceiveRequest();
                    messageResponse = JsonConvert.DeserializeObject<Request>(messageReceived);
                    Console.WriteLine(messageResponse.Command);
                    break;
                case "mailbox":
                    // messages receive
                    var mailboxResponse = JsonConvert.DeserializeObject<MailsResponse>(newMessage);
                    Console.WriteLine(mailboxResponse.Message);
                    if (mailboxResponse.Mails.Count > 0)
                    {
                        foreach (var message in mailboxResponse.Mails)
                        {
                            Console.WriteLine(message);
                        }
                    }
                    else
                        Console.WriteLine("Your mailbox is empty.");
                    break;
                case "stop":
                    exchangeOn = false;
                    break;
                case "users":
                    var usersResponse = JsonConvert.DeserializeObject<UsersResponse>(newMessage);
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    Console.WriteLine("                  " + usersResponse?.Message);
                    Console.WriteLine("\n######################################################\n");
                    foreach (var user in usersResponse?.Users)
                    {
                        Console.WriteLine($"{user} ");
                    }
                    Console.WriteLine("\n######################################################");
                    Console.WriteLine("######################################################\n");
                    break;
                default:
                    var wrongResponse = JsonConvert.DeserializeObject<Request>(newMessage);
                    Console.WriteLine(wrongResponse.Command);
                    break;
            }
        }
    }
}
