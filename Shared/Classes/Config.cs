using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    public class Config
    {
        public static IPAddress IpAddr { get; } = IPAddress.Parse("127.0.0.1");
        public static IPEndPoint LocalEndPoint { get; } = new(IpAddr, 9013);

    }
}
