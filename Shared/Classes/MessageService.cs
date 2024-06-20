using Newtonsoft.Json;
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
        private string _messagesPath;

        public MessageService(IUserManagementService userManagementService, string messagesPath)
        {
            _userManagementService = userManagementService;
            _messagesPath = messagesPath;
            usersMessages = LoadMessages();
        }

        public List<Message> GetMessages()
        {
            var currentUser = _userManagementService.GetUser();
            List<Message> messages = new List<Message>();
            if (!ValidateRecipient(currentUser.Login))
                return messages;
            if (usersMessages.ContainsKey(currentUser.Login))
                messages = usersMessages[currentUser.Login];
            Console.WriteLine(messages);
            return messages;

            //foreach (var message1 in usersMessages)
            //{
            //    Console.WriteLine($"Messages for {message1.Key}:");
            //    foreach (var message2 in message1.Value)
            //    {
            //        Console.WriteLine($"Sender: {message2.Sender}, Content: {message2.Content}, CreationDate&Time: {message2.MessageCreationDateTime}");
            //    }
            //}
        }

        public string SendMessage(string recipient, string message)
        {
            LoadMessages();
            var currentUser = _userManagementService.GetUser();
            if (!ValidateRecipient(recipient))
                return $"Sending failed. User {recipient} doesn't exist.";
            if (!ValidateMessage(message))
                return $"Sending failed. Message is too long.";
            if (!CheckFullMailbox(recipient))
                return $"Sending failed. Mailbox is full.";
            Message singleUserMessages = new Message(currentUser.Login, message);
            var userExistInMailbox = false;
            foreach (var userMailbox in usersMessages)
            {
                if (userMailbox.Key == recipient)
                {
                    userMailbox.Value.Add(singleUserMessages);
                    userExistInMailbox = true;
                    break;
                }
            }
            if (!userExistInMailbox)
            {
                var messages = new List<Message>();
                messages.Add(singleUserMessages);
                usersMessages.Add(recipient, messages);
            }
            SaveMessages(usersMessages);
            return $"Message has been sent to {recipient}.";
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

        private Dictionary<string, List<Message>> LoadMessages()
        {
            if (!File.Exists(_messagesPath))
            {
                Console.WriteLine("test");
                return new Dictionary<string, List<Message>>();
            }

            using (var reader = new StreamReader(_messagesPath))
            {
                var json = reader.ReadToEnd();

                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new UserConverter() }
                };

                return JsonConvert.DeserializeObject<Dictionary<string, List<Message>>>(json, settings) ?? new Dictionary<string, List<Message>>();
            }
        }

        private void SaveMessages(Dictionary<string, List<Message>> usersMessages)
        {
            try
            {
                using (var writer = new StreamWriter(_messagesPath))
                {
                    var json = JsonConvert.SerializeObject(usersMessages, Formatting.Indented);
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private bool CheckFullMailbox(string name)
        {
            if (usersMessages.ContainsKey(name))
            {
                if (usersMessages[name].Count >= 5)
                    return false;
            }
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
