using Moq;
using Shared.Classes.Repositories.FileType;
using Shared.Classes.Services;
using Shared.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class UserManagementServiceTest
    {
        // temporary file path
        private const string UsersFilePath = "test_users.json";

        public UserManagementServiceTest()
        {
            if (File.Exists(UsersFilePath))
            {
                File.Delete(UsersFilePath);
            }
        }

        [Fact]
        public void RegisterUserWhenUserDoesNotExistShouldRegisterUser()
        {
            // Arrange
            string login = "testuser";
            string password = "testpassword";

            IUserRepository userRepository = new UserFileRepository(UsersFilePath);
            var userService = new UserManagementService(userRepository);

            // Act
            string result = userService.RegisterUser(login, password);

            // Assert
            Assert.Contains($"User: {login} has been created.", result);
        }

        [Fact]
        public void RegisterUserWhenUserExistShouldReject()
        {
            // Arrange
            string login = "admin";
            string password = "admin123";

            IUserRepository userRepository = new UserFileRepository(UsersFilePath);
            var userService = new UserManagementService(userRepository);

            // Act
            string result = userService.RegisterUser(login, password);

            // Assert
            Assert.Contains("User already exists", result);
        }
    }
}
