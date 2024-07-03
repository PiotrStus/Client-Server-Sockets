using Newtonsoft.Json;
using Shared.Classes;
using Shared.Classes.Repositories.FileType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class MessageFileRepositoryTests
    {
        private const string TestFilePath = "testfile.json";

        [Fact]
        public void GetAllMessagesShouldReturnEmptyDictionaryWhenFileDoesNotExist()
        {
            // Arrange
            var messagesPath = "nonexistentfile.json";
            var repository = new MessageFileRepository(messagesPath);


            // Act
            var result = repository.GetAllMesasges();


            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public void GetAllMessagesShouldReturnParsedMessagesWhenFileExists()
        {
            // Arrange
            var expectedMessages = new Dictionary<string, List<Message>>
            {
                { "userRecipient", new List<Message> { new Message("userSender", "Hello World") } }
            };
            var json = JsonConvert.SerializeObject(expectedMessages, Formatting.Indented);

            File.WriteAllText(TestFilePath, json);

            var repository = new MessageFileRepository(TestFilePath);

            // Act
            var result = repository.GetAllMesasges();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("userRecipient", result.Keys);
            var messages = result["userRecipient"];
            Assert.Single(messages);
            var message = messages.First();
            Assert.Equal("userSender", message.Sender);
            Assert.Equal("Hello World", message.Content);
        }


        [Fact]
        public void SaveMessages_ShouldWriteToFile()
        {
            // Arrange
            var repository = new MessageFileRepository(TestFilePath);
            var messages = new Dictionary<string, List<Message>>
            {
                {"user1", new List<Message> {new Message("user2", "What's going on?")} }
            };

            // Act

            repository.SaveMessages(messages);


            // Assert
            string fileContent;
            using (var reader = new StreamReader(TestFilePath)) {
                fileContent = reader.ReadToEnd();
            }
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }

            var deserializedMessage = JsonConvert.DeserializeObject<Dictionary<string, List<Message>>> (fileContent);
            //var fileContent = File.ReadAllText(TestFilePath);


            Assert.NotNull(fileContent);
            Assert.Contains("user1", deserializedMessage!.Keys);

            var message = deserializedMessage!["user1"].First();

            Assert.Equal("What's going on?", message.Content);

        }
    }
}
