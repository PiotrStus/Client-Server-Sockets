using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    public class Message
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public Message(string sender, string content)
        {
            Sender = sender;
            Content = content;
        }
    }
}
