using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Responses
{
    public class HelpResponse
    {
        public string? Message { get; set; }
        public List<string>? Commands { get; set; }
    }
}
