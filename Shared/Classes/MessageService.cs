using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    public class MessageService : IMessageService
    {
        private Dictionary<string, List<Message>> usersMessages = new Dictionary<string, List<Message>>();


        private IUserManagementService _userManagementService;
        public MessageService(IUserManagementService userManagementService, string messagePath)
        {
            _userManagementService = userManagementService;
        }
        public List<Message> GetMessage(User user)
        {
            throw new NotImplementedException();
        }

        public string SendMessage(string recipient, string message)
        {
            var currentUser = _userManagementService.GetUser();
            if (!ValidateRecipient(recipient))
                return $"Sending failed. User {recipient} doesn't exist";
            if (!ValidateMessage(message))
                return $"Sending failed. Message is too long.";



            List<Message> singleUserMessages = new List<Message>();
            singleUserMessages.Add(new Message(recipient, message));
            usersMessages.Add(currentUser.Login, singleUserMessages);
            
            
            
            


            foreach (var message1 in usersMessages)
            {
                Console.WriteLine($"Messages for {message1.Key}:");
                foreach (var message2 in message1.Value)
                {
                    Console.WriteLine($"Sender: {message2.Sender}, Content: {message2.Content}, CreationDate&Time: {message2.MessageCreationDateTime}");
                }
            }




            return "Sending in progress...";
        }

        private bool ValidateRecipient(string recipient)
        {
            var users = _userManagementService.GetAllUsers();
            foreach (var user in users) 
            {
                if (user.Login == recipient)
                    return true;
            }
            return false;
        }

        private bool ValidateMessage(string message)
        {
            if (message.Length > 255)
            {
                //Console.WriteLine(message.Length);
                return false;
            }
            //Console.WriteLine(message.Length);
            return true;
        }







        public void Test()
        {
            var users = _userManagementService.GetAllUsers();
            foreach (var user in users)
            {
                Console.WriteLine(user.Login);
            }
        }
    }
}
