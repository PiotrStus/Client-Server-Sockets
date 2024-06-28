using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Responses;
using Shared.Requests;

namespace Client.Handlers
{
    public class ResponseHandler
    {
        private readonly ICommunicationService _communicationService;
        public ResponseHandler(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        public void HandleInfo(string newMessage)
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

        public void HandleRegister(string newMessage)
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

        public void HandleLogin(string newMessage)
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

        public void HandleLogout(string newMessage)
        {
            var logoutResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLogout(logoutResponse.Command);
        }

        public void HandleDelete(string newMessage)
        {
            var deleteResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLine(deleteResponse.Command);

            string userInput = GetUserInput();
            _communicationService.SendResponse(CreateRequest(userInput));

            deleteResponse = ReceiveAndDeserialize<Request>();
            CommunicationMessages.ShowLine(deleteResponse.Command);
        }

        public void HandleUptime(string newMessage)
        {
            var uptimeResponse = JsonConvert.DeserializeObject<UptimeResponse>(newMessage);
            CommunicationMessages.ShowUptime(uptimeResponse.Message, uptimeResponse.UpTime);
        }

        public void HandleHelp(string newMessage)
        {
            var helpResponse = JsonConvert.DeserializeObject<HelpResponse>(newMessage);
            CommunicationMessages.ShowHelp(helpResponse.Message, helpResponse.Commands);
        }

        public void HandleMessage(string newMessage)
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

        public void HandleMailbox(string newMessage)
        {
            var mailboxResponse = JsonConvert.DeserializeObject<MailsResponse>(newMessage);
            CommunicationMessages.ShowMailbox(mailboxResponse.Message, mailboxResponse.Mails);
        }

        public void HandleUsers(string newMessage)
        {
            var usersResponse = JsonConvert.DeserializeObject<UsersResponse>(newMessage);
            CommunicationMessages.ShowUsers(usersResponse.Message, usersResponse.Users);
        }

        public void HandleDefault(string newMessage)
        {
            var wrongResponse = JsonConvert.DeserializeObject<Request>(newMessage);
            CommunicationMessages.ShowLine(wrongResponse.Command);
        }
    }
}
