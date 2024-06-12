using Shared.Classes;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal class Program
    {
        private static string? jsonMsg;
        private static bool exchangeOn = true;
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9013);
            Socket clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            RegularUser userTest = new RegularUser("paweł", "gaweł");
            userTest.RegisterUser();
            try
            {
                clientSocket.Connect(endPoint);
                Console.WriteLine("######################################");
                Console.WriteLine("Conntected to the server.");
                Console.WriteLine("Socket connected to -> {0} ", clientSocket.RemoteEndPoint!.ToString());
                Console.WriteLine("######################################\n");
                byte[] messageReceived = new byte[1024];
                int byteReceived = clientSocket.Receive(messageReceived);
                Console.WriteLine("New message from Server: \n\n{0}",
                Encoding.ASCII.GetString(messageReceived,
                                           0, byteReceived));
               
                
                
                
                while (exchangeOn)
                {
                    Console.Write("\nEnter a new command: ");
                    string userInput = Console.ReadLine()!;
                    if (!string.IsNullOrEmpty(userInput))
                    {
                        Console.Clear();
                        byte[] messageSent = Encoding.ASCII.GetBytes(userInput);
                        int byteSent = clientSocket.Send(messageSent);
                        byteReceived = clientSocket.Receive(messageReceived);
                        string newMessage = Encoding.ASCII.GetString(messageReceived, 0, byteReceived);
                        jsonMsg = JsonSerializer.Deserialize<string>(newMessage);
                        if (jsonMsg != "register")
                        {
                            Console.WriteLine("New message from Server: \n\n{0}", jsonMsg);
                        }
                        if (jsonMsg == "stop")
                            exchangeOn = false;
                        else if (jsonMsg == "register")
                        {
                            Console.WriteLine("Please type your login: ");
                            userInput = Console.ReadLine()!;
                            messageSent = Encoding.ASCII.GetBytes(userInput);
                            jsonMsg = JsonSerializer.Serialize(userInput);
                            clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
                            Array.Clear(messageSent, 0, messageSent.Length);
                            Console.Clear();
                            Console.WriteLine("Please type your password: ");
                            userInput = Console.ReadLine()!;
                            jsonMsg = JsonSerializer.Serialize(userInput);
                            clientSocket.Send(Encoding.ASCII.GetBytes(jsonMsg));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid command");
                    }
                }





                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't establish connection to the server: " + ex.ToString());
            }
        }
    }
}
