using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IMessageService
    {
        /// <summary>
        /// Sends a message from one user to another.
        /// </summary>
        /// <param name="recipient">The user who will receive the message.</param>
        /// <param name="message">The message to be sent.</param>
        string SendMessage(string recipient, string message);
        /// <summary>
        /// Check all messages for a specific user
        /// </summary>
        /// <param name="user">The user which messages should be checked.</param>
        List<Message> GetMessages();
        void Test();
    }
}
