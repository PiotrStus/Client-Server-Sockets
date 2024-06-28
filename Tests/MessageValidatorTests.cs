using Shared.Classes.Validators;
using Shared.Interfaces;
using Shared.Classes;
using Shared.Classes.Services;
using Shared.Interfaces.Repository;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

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

        [Theory]
        [InlineData("user1")]
        [InlineData("user2")]
        public void ValidateRecipientValidRecipientReturnsTrue(string existingRecipient)
        {
            // Arrange
            //string existingRecipient = "user1";

            // Act
            bool isValid = _validator.ValidateRecipient(existingRecipient);

            // Assert
            Assert.True(isValid);
        }


        [Fact]
        public void ValidateRecipientInvalidRecipientReturnsFalse()
        {
            // Arrange
            string existingRecipient = "unkown_user";

            // Act
            bool isValid = _validator.ValidateRecipient(existingRecipient);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateMessageMessageValidReturnsTrue()
        {
            string newMessage = "test message";

            bool isValid = _validator.ValidateMessage(newMessage);

            Assert.True(isValid);

        }

        [Fact]
        public void ValidateMessageMessageInvalidReturnsFalse()
        {
            string newMessage = "...................Salve amice! Spero te bene valere. Hodie mihi fortuna favet. Sol splendet," +
                " et ventus lenis spirat. Vita pulchra est! Hoc tempore studeo, ut meliorem faciam. Gratias " +
                "tibi ago pro amicitia tua. Vale!........................................";

            bool isValid = _validator.ValidateMessage(newMessage);

            Assert.False(isValid);

        }


        [Fact]
        public void CheckMailboxFullMailboxFullReturnsTrue()
        {
            var usersMessages = new Dictionary<string, List<Message>>
            {
                { "user2", new List<Message>
                    {
                        new Message("sender1", "Message 1 content"),
                        new Message("sender2", "Message 2 content"),
                        new Message("sender3", "Message 3 content"),
                        new Message("sender1", "Message 4 content"),
                        new Message("sender2", "Message 5 content"),
                        new Message("sender3", "Message 6 content")
                    } 
                }
            };
            string userName = "user2";


            bool isNotFull = _validator.CheckFullMailbox(usersMessages, userName);
            Assert.True(!isNotFull);
        }

        [Fact]
        public void CheckMailboxFullMailboxNotFullReturnsTrue()
        {
            var usersMessages = new Dictionary<string, List<Message>>
            {
                { "user2", new List<Message>
                    {
                        new Message("sender1", "Message 1 content"),
                    }
                }
            };
            string userName = "user2";


            bool isNotFull = _validator.CheckFullMailbox(usersMessages, userName);
            Assert.True(isNotFull);
        }


    }
}