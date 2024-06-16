using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Responses
{
    public class InfoResponse
    {
        public string? Message { get; set; }
        public DateTime ServerCreated { get; set; }
        public string? ServerVersion { get; set; }
    }
}
