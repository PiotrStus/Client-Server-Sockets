using Shared.Classes.Validators;
using Shared.Interfaces;
using Shared.Classes;
using Shared.Classes.Services;
using Shared.Interfaces.Repository;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Tests
{
    public class MessageValidatorTests
    {

        private readonly Mock<IUserManagementService> _mockUserManagementService;
        private readonly MessageValidator _validator;

        public MessageValidatorTests()
        {
            _mockUserManagementService = new Mock<IUserManagementService>();

            // Przygotowanie mocka IUserManagementService do zwracania przyk³adowych u¿ytkowników
            _mockUserManagementService.Setup(service => service.GetAllUsers())
                .Returns(new List<User>
                {
                    new RegularUser("user1", "password1"),
                    new RegularUser ("user2", "password2")
                });

            _validator = new MessageValidator(_mockUserManagementService.Object, new Dictionary<string, List<Message>>());
        }

        [Fact]
        public void ValidateRecipient_ValidRecipient_ReturnsTrue()
        {
            // Arrange
            string existingRecipient = "user1";

            // Act
            bool isValid = _validator.ValidateRecipient(existingRecipient);

            // Assert
            Assert.True(isValid);
        }


        [Fact]
        public void ValidateRecipient_InvalidRecipient_ReturnsFalse()
        {
            // Arrange
            string existingRecipient = "unkown_user";

            // Act
            bool isValid = _validator.ValidateRecipient(existingRecipient);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateMessage_MessageValid_ReturnsTrue()
        {
            string newMessage = "test message";

            bool isValid = _validator.ValidateMessage(newMessage);

            Assert.True(isValid);

        }

    }
}