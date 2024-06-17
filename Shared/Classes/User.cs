using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Classes
{
    public abstract class User
    {
        public User(string login, string password)
        {
            Password = password;
            Login = login;
        }

        public abstract Constants.UserTypes Type { get; }
        public string Login { get; private set; }
        public string Password { get; private set; }
    }
}
