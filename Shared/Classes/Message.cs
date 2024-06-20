using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    public class Message
    {
        public string Sender { get;  private set; }
        public string Content { get; private set; }
        public DateTime MessageCreationDateTime;

        public Message(string sender, string content)
        {
            Sender = sender;
            Content = content;
            MessageCreationDateTime = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Sender: {Sender}, Content: {Content}, CreationDate&Time: {MessageCreationDateTime:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
