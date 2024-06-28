using Client.Handlers;
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

        public delegate void ResponseHandlerDelegate(string newMessage);
        private readonly Dictionary<string, ResponseHandlerDelegate> _responseHandlers;
        private readonly ResponseHandler _responseHandler;

        public Client(ICommunicationService communicationService)
        {
            _communicationService = communicationService;

            _responseHandler = new ResponseHandler(communicationService);

            _responseHandlers = new Dictionary<string, ResponseHandlerDelegate>
            {
                {"info", _responseHandler.HandleInfo },
                {"register", _responseHandler.HandleRegister },
                {"login", _responseHandler.HandleLogin },
                {"logout", _responseHandler.HandleLogout },
                { "delete", _responseHandler.HandleDelete },
                { "uptime", _responseHandler.HandleUptime },
                { "help", _responseHandler.HandleHelp },
                { "message", _responseHandler.HandleMessage },
                { "mailbox", _responseHandler.HandleMailbox },
                { "users", _responseHandler.HandleUsers },
                { "stop", (msg) => _exchangeOn = false }, 
                { "default", _responseHandler.HandleDefault }
            };
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
                        HandleResponse(newMessage, command);
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

        

        public void HandleResponse(string newMessage, string command)
        {
            if(_responseHandlers.TryGetValue(command, out ResponseHandlerDelegate handler))
            {
                handler(newMessage);
            }
            else
            {
                _responseHandler.HandleDefault(newMessage);
            }
        }
    }
}
