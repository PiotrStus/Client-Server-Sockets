using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    public class MessageValidator
    {
        private IUserManagementService _userManagementService;
        public MessageValidator(IUserManagementService userManagementService, Dictionary<string, List<Message>> usersMessages)
        {
            _userManagementService = userManagementService;
        }

        public bool ValidateRecipient(string recipient)
        {
            var users = _userManagementService.GetAllUsers();
            foreach (var user in users)
            {
                if (user.Login == recipient)
                    return true;
            }
            return false;
        }
        public bool ValidateMessage(string message)
        {
            if (message.Length > 255)
            {
                return false;
            }
            return true;
        }
        public bool CheckFullMailbox(Dictionary<string, List<Message>> usersMessages, string name)
        {
            if (usersMessages.ContainsKey(name))
            {
                if (usersMessages[name].Count >= 5)
                    return false;
            }
            return true;
        }

    }
}
