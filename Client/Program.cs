using Shared.Classes;
using Shared.Classes.Services;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Shared.Responses;
using Shared.Requests;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(Config.IpAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(Config.LocalEndPoint);
            ConnectionInfoPresenter.ShowConnectionDetails(clientSocket);
            var communicationService = new SocketCommunicationService(clientSocket);
            Client client = new Client(communicationService);
            client.Start();
        }
    }
}
