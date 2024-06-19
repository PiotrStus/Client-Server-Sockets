using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    public class Message
    {
        private User _sernder;
        private User _recipient;
        private string _content;
        private DateTime _timeStamp;

        public Message(User sender, User recipient, string content)
        {
            _sernder = sender;
            _recipient = recipient;
            _content = content;
            _timeStamp = DateTime.Now;
        }
    }
}
