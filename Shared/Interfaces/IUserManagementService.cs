using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IUserManagementService
    {
        string RegisterUser(string login, string password);
        string RegisterAdmin(string login, string password);
        User? LoginUser(string login, string password);
        string LogoutUser();
        string DeleteUser(string login);
        User? GetUser();
        bool IsAdmin();
        List<User>? GetAllUsers();
    }
}
