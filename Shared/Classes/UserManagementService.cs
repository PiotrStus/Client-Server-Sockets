using Newtonsoft.Json;
using Shared.Classes.Shared.Classes;
using Shared.Interfaces;
using Shared.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Classes
{
    public class UserManagementService : IUserManagementService
    {
        private List<User> users = new List<User>();
        private readonly string _usersPath;
        public bool UserIsAdmin { get; private set; }
        public User CurrentUser { get; private set; }
        public UserManagementService(string filePath)
        {
            _usersPath = filePath;
            users = LoadUsers();
            var x = RegisterAdmin("admin", "admin123");
        }
        public string DeleteUser(string login)
        {
            var user = LoadUsers();
            var userToRemove = users.FirstOrDefault(x => x.Login == login);
            if (userToRemove == null)
            {
                return "Deleting user failed";
            }
            users.Remove(userToRemove);
            SaveUsers(users);
            return $"User {login} has been deleted.";
        }
        public List<User> GetAllUsers()
        {
            return users;
        }
        public User? LoginUser(string login, string password)
        {
            var user = users.Find(u => u.Login == login && u.Password == password);

            if (user != null)
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
            if (users.Any(u => u.Login == login))
            {
                return "User already exists";
            }
            var user = new RegularUser(login, password);
            users.Add(user);
            SaveUsers(users);
            return $"User: {login} created.";
        }
        public string RegisterAdmin(string login, string password)
        {
            if (users.Any(u => u.Login == login))
            {
                return "User already exists";
            }
            var user = new AdminUser(login, password);
            users.Add(user);
            SaveUsers(users);
            return $"Admin: {login} created.";
        }
        private List<User> LoadUsers()
        {
            if (!File.Exists(_usersPath))
            {
                return new List<User>();
            }
            using (var reader = new StreamReader(_usersPath))
            {
                var json = reader.ReadToEnd();
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new UserConverter() }
                };
                return JsonConvert.DeserializeObject<List<User>>(json, settings) ?? new List<User>();
            }
        }
        private void SaveUsers(List<User> users)
        {
            try
            {
                using (var writer = new StreamWriter(_usersPath))
                {
                    var json = JsonConvert.SerializeObject(users, Formatting.Indented);
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
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