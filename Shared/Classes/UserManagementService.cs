using Newtonsoft.Json;
using Shared.Classes.Shared.Classes;
using Shared.Interfaces;
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
        
        private readonly string _filePath ;
        
        public UserManagementService(string filePath) 
        {
            _filePath = filePath;
            users = LoadUsers();
            var x = RegisterAdmin("adam", "gadam");
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
            foreach (var user in users) 
                { 
                Console.WriteLine(user.Login);
            }
            return users;
        }

        public User? LoginUser(string login, string password)
        {
            return users.Find(u => u.Login == login && u.Password == password);
        }

        public string LogoutUser()
        {
            throw new NotImplementedException();
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
            if (!File.Exists(_filePath))
            {
                return new List<User>();
            }

            var json = File.ReadAllText(_filePath);
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new UserConverter() }
            };
            return JsonConvert.DeserializeObject<List<User>>(json, settings) ?? new List<User>();
        }
        private void SaveUsers(List<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
