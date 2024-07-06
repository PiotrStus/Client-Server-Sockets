using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class CommunicationMessages
    {
        public static void ShowInfo(string message, DateTime serverCreated, string serverVersion)
        {
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
            Console.WriteLine("                 " + message);
            Console.WriteLine("\n######################################################\n");
            Console.WriteLine($"Server's creation date&time: {serverCreated}");
            Console.WriteLine($"Server's version: {serverVersion}");
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
        }
        public static void ShowLine(string command)
        {
            Console.WriteLine(command);
        }
        public static void ShowLogout(string command)
        {
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
            Console.WriteLine("             " + command);
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
        }

        public static void ShowUptime(string message, string uptime)
        {
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
            Console.WriteLine("              " + message);
            Console.WriteLine("\n######################################################\n");
            Console.WriteLine($"                        {uptime}");
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
        }

        public static void ShowHelp(string message, List<string> commands)
        {
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
            Console.WriteLine("                  " + message);
            Console.WriteLine("\n######################################################\n");
            foreach (var availableCommand in commands)
            {
                Console.WriteLine($"          {availableCommand}");
            }
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
        }

        public static void ShowMailbox(string message, List<Message> mails)
        {
            Console.WriteLine(message);
            if (mails.Count > 0)
            {
                foreach (var mail in mails)
                {
                    Console.WriteLine(mail.Content);
                    Console.WriteLine(mail.MessageCreationDateTime);
                    Console.WriteLine("from " + mail.Sender);
                }
            }
            else
            {
                Console.WriteLine("Your mailbox is empty.");
            }
        }

        public static void ShowUsers(string message, List<string> users)
        {
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
            Console.WriteLine("                  " + message);
            Console.WriteLine("\n######################################################\n");
            foreach (var user in users)
            {
                Console.WriteLine($"{user}");
            }
            Console.WriteLine("\n######################################################");
            Console.WriteLine("######################################################\n");
        }
    }
}
