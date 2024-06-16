using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new Server();
            await server.Start();
        }
    }
}
