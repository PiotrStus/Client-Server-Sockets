using Newtonsoft.Json;
using Shared.Classes.Shared.Classes;
using Shared.Interfaces;
using Shared.Interfaces.Repository;
using Shared.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared.Classes.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        public bool UserIsAdmin { get; private set; }
        public User CurrentUser { get; private set; }
        public UserManagementService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            var x = RegisterAdmin("admin", "admin123");
        }

        public string DeleteUser(string login)
        {
            var user = _userRepository.GetUser(login);
            if (user == null)
            {
                return "Deleting user failed";
            }
            _userRepository.DeleteUser(login);
            return $"User {login} has been deleted.";
        }
        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }
        public User? LoginUser(string login, string password)
        {
            var user = _userRepository.GetUser(login);

            if (user != null && user.Login == login && user.Password == password)
            {
                CurrentUser = user;
                UserIsAdmin = CurrentUser.Type == Constants.UserTypes.Admin;
                return CurrentUser;
            }
            return null;
        }
        public string LogoutUser()
        {
            var response = "No user is currently logged in";
            if (CurrentUser != null)
            {
                response = $"User - {CurrentUser.Login} logout successful";
                CurrentUser = null;
            }
            UserIsAdmin = UserIsAdmin ? false : UserIsAdmin;
            return response;
        }
        public string RegisterUser(string login, string password)
        {
            var user = _userRepository.GetUser(login);
            if (user != null)
            {
                return "User already exists";
            }
            user = new RegularUser(login, password);
            _userRepository.AddUser(user);
            return $"User: {login} has been created.";
        }
        public string RegisterAdmin(string login, string password)
        {
            var user = _userRepository.GetUser(login);
            if (user != null)
            {
                return "User already exists";
            }
            user = new AdminUser(login, password);
            _userRepository.AddUser(user);
            return $"Admin: {login} has been created.";
        }

        public User? GetUser()
        {
            return CurrentUser;
        }
        public bool IsAdmin()
        {
            return UserIsAdmin;
        }
    }
}