using Newtonsoft.Json;
using Shared.Classes;
using Shared.Classes.Converters;
using Shared.Classes.Repositories.FileType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class UserFileRepositoryTests
    {

        private const string TestFilePath = "testuser.json";


        [Fact]
        public void GetAllUsersShouldReturnEmptyListWhenFileDoesNotExist()
        {
            // Arrange
            var repository = new UserFileRepository(TestFilePath);

            // Act
            var result = repository.GetAllUsers();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]

        public void AddUserShouldaAddUserEventIfListIsEmpty()
        {
            // Arrange
            var repository = new UserFileRepository(TestFilePath);
            var newUser = new RegularUser("testUser", "testPassword");

            // Act
            repository.AddUser(newUser);

            // Assert
            var users = ReadUsersFromFile();
            var userFound = users.Any(u => u.Login == newUser.Login && u.Password == newUser.Password);
            Assert.True(userFound);
        }

        private List<User> ReadUsersFromFile()
        {
            if (!File.Exists(TestFilePath))
                return new List<User>();

            var json = File.ReadAllText(TestFilePath);
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new UserConverter() }
            };
            return JsonConvert.DeserializeObject<List<User>>(json, settings) ?? new List<User>();
        }


        [Fact]
        public void DeleteUserShouldDeleteFromList()
        {
            // Arrange
            var repository = new UserFileRepository(TestFilePath);
            var newUser = new RegularUser("userToDelete", "passwordToDelete");
            repository.AddUser(newUser);

            // Act
            repository.DeleteUser(newUser.Login);

            // Assert
            var users = ReadUsersFromFile();
            var userDeleted = users.All(user =>  user.Login != newUser.Login);

            Assert.True(userDeleted);

        }

    }
}
