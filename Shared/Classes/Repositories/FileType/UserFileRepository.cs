using Newtonsoft.Json;
using Shared.Interfaces.Repository;
using Shared.Classes.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Shared.Classes.Repositories.FileType
{
    public class UserFileRepository : IUserRepository
    {
        private readonly string _usersPath;

        public UserFileRepository(string usersPath)
        {
            _usersPath = usersPath;
        }

        public void AddUser(User user)
        {
            var users = LoadUsers();
            if (users.All(u => u.Login != user.Login))
            {
                users.Add(user);
            }
            SaveUsers(users);
        }

        public void DeleteUser(string login)
        {
            var users = LoadUsers();
            var userToRemove = users.Find(u => u.Login == login);
            if (userToRemove != null)
            {
                users.Remove(userToRemove);
                SaveUsers(users);
            }
        }

        public List<User> GetAllUsers()
        {
            return LoadUsers();
        }

        public User? GetUser(string login)
        {
            var users = LoadUsers();
            return users.Find(u => u.Login == login);
        }

        public void UpdateUser(User user)
        {
            var users = LoadUsers();
            var existingUser = users.Find(u => u.Login == user.Login);
            if (existingUser != null)
            {
                users.Remove(existingUser);
                users.Add(user);
                SaveUsers(users);
            }
        }



        // helpers methods

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
                Console.WriteLine($"Error saving users to file: {ex.Message}");
            }
        }

    }
}
