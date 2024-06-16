using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using global::Shared.Interfaces;

    namespace Shared.Classes
    {
        public class AdminUser : User, IFileReader, IFileWriter
        {
            [JsonIgnore]
            public string FilesDirectory { get; private set; }
            public override Constants.UserTypes Type
            {
                get { return Constants.UserTypes.Admin; }
            }

            public AdminUser(string username, string password) : base(username, password)
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

}
