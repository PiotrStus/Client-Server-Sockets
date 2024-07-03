using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class MessageTests
    {

        [Fact]
        public void ConstructorShouldInitializeProperties()
        {

            // Arrange
            var sender = "TestSender";
            var content = "TestContent";

            // Act

            var message = new Message(sender, content);


            // Assert
            Assert.Equal(sender, message.Sender);
            Assert.Equal(content, message.Content);
        }

        [Fact]
        public void ToStringShouldReturnFormattedString()
        {
            // Arrange
            var sender = "user1";
            var content = "Hi, how are you?";
            var message = new Message(sender, content);
            var expectedOutput = $"Sender: {sender}, Content: {content}, CreationDate&Time: {message.MessageCreationDateTime:yyyy-MM-dd HH:mm:ss}";

            var result = message.ToString();

            Assert.Equal(expectedOutput, result);

        }

    }
}
