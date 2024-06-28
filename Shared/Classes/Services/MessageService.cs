using Newtonsoft.Json;
using Shared.Classes.Validators;
using Shared.Classes.Converters;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Interfaces.Repository;

namespace Shared.Classes.Services
{
    public class MessageService : IMessageService
    {
        private Dictionary<string, List<Message>> usersMessages = new Dictionary<string, List<Message>>();
        private IUserManagementService _userManagementService;
        private IMessageRepository _messageRepository;
        private MessageValidator _messageValidator;
        public MessageService(IUserManagementService userManagementService, IMessageRepository messageRepository)
        {
            _userManagementService = userManagementService;
            _messageRepository = messageRepository;
            usersMessages = _messageRepository.GetAllMesasges();
            _messageValidator = new MessageValidator(_userManagementService, usersMessages);
        }
        public List<Message> GetUserMessages()
        {
            var currentUser = _userManagementService.GetUser();
            List<Message> messages = new List<Message>();
            if (!_messageValidator.ValidateRecipient(currentUser.Login))
                return messages;
            if (usersMessages.ContainsKey(currentUser.Login))
                messages = usersMessages[currentUser.Login];
            return messages;
        }
        public string SendMessage(string recipient, string message)
        {
            var currentUser = _userManagementService.GetUser();

            if (!_messageValidator.ValidateRecipient(recipient))
                return $"Sending failed. User {recipient} doesn't exist.";
            if (!_messageValidator.ValidateMessage(message))
                return $"Sending failed. Message is too long.";
            if (!_messageValidator.CheckFullMailbox(usersMessages, recipient))
                return $"Sending failed. Mailbox is full.";
            Message singleUserMessage = new Message(currentUser.Login, message);
            var userExistInMailbox = false;

            if (usersMessages.ContainsKey(recipient))
            {
                usersMessages[recipient].Add(singleUserMessage);
                userExistInMailbox = true;
            }
            if (!userExistInMailbox)
            {
                usersMessages.Add(recipient, new List<Message> { singleUserMessage });
            }

            _messageRepository.SaveMessages(usersMessages);
            return $"Message has been sent to {recipient}.";
        }
    }
}
