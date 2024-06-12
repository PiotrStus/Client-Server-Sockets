using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private string Login { get; set; }
        private string Password { get; set; }
        public virtual void RegisterUser()
        {
        }
        public virtual void LoginUser()
        {
        }
    }
}
