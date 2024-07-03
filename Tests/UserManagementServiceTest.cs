using Moq;
using Shared.Classes;
using Shared.Classes.Repositories.FileType;
using Shared.Classes.Services;
using Shared.Classes.Shared.Classes;
using Shared.Interfaces;
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

        [Fact]
        public void LoginUserWithValidCredentialsShouldLoginSuccessfully()
        {
            // Arrange
            var login = "testuser";
            var password = "testpassword";

            var mockUserRepository = new Mock<IUserRepository>();

            mockUserRepository.Setup(repo => repo.GetUser(login))
                              .Returns(new RegularUser("testuser", "testpassword"));

            var userService = new UserManagementService(mockUserRepository.Object);

            // Act
            var result = userService.LoginUser(login, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(login, result.Login);
            Assert.Equal(password, result.Password);

        }

        [Fact]
        public void LoginUserWithInvalidCredentialsShouldNotLoginSuccessfully()
        {
            // Arrange
            var login = "testuser";
            var password = "testpassword";

            var mockUserRepository = new Mock<IUserRepository>();

            mockUserRepository.Setup(repo => repo.GetUser(login))
                              .Returns(new RegularUser("testuser", "testpassword1"));

            var userService = new UserManagementService(mockUserRepository.Object);

            // Act
            var result = userService.LoginUser(login, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]

        public void LoginAdminShouldReturnTypeAdmin()
        {
            // Arrange

            var login = "admin";
            var password = "admin123";

            var mockUserRepository = new Mock<IUserRepository>();

            mockUserRepository.Setup(repo => repo.GetUser(login))
                              .Returns(new AdminUser("admin", "admin123"));

            var userService = new UserManagementService(mockUserRepository.Object);


            // Act
            
            var result = userService.LoginUser(login, password);

            // Assert

            Assert.Equal("Admin", result.Type.ToString());

        }


    }
}
