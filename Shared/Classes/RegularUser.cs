using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Interfaces;
using Newtonsoft.Json;

namespace Shared.Classes
{
    public class RegularUser : User, IFileReader, IFileWriter
    {
        [JsonIgnore]
        public string FilesDirectory { get; private set; }
        public override Constants.UserTypes Type 
        {
            get { return Constants.UserTypes.RegularUser; }
        }

        public RegularUser(string username, string password) : base(username, password)
        {
           //GetFileDirectory();
        }

        public void RegisterUser(string username, string password) 
        {
        }


        public void GetFileDirectory() 
        {
            try
            {
                FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserFiles");
                if (!Directory.Exists(FilesDirectory))
                {
                    Directory.CreateDirectory(FilesDirectory);
                    Console.WriteLine("Folder created");
                }
                else
                {
                    Console.WriteLine($"Folder - {FilesDirectory} already exists.\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public void WriteToFile(string filePath, string fileData)
        {
            Console.WriteLine("Write Test");
        }

        public void ReadFromFile(string filePath)
        {
            Console.WriteLine("Read Test");
        }

    }
}
