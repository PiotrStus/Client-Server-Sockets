using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shared.Classes
{
    public class RegularUser : User
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
    }
}
