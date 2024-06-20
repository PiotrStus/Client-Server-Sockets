using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Responses
{
    public class MailsResponse
    {
        public string? Message { get; set; }
        public List<Message>? Mails { get; set; }
    }
}
