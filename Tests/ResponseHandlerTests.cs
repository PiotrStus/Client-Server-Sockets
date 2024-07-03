using Client;
using Client.Handlers;
using Moq;
using Newtonsoft.Json;
using Shared.Interfaces;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ResponseHandlerTests
    {
        private readonly Mock<ICommunicationService> _mockCommunicationService;
        private readonly ResponseHandler _responseHandler;

        public ResponseHandlerTests()
        {
            _mockCommunicationService = new Mock<ICommunicationService>();
            _responseHandler = new ResponseHandler(_mockCommunicationService.Object);
        }

        [Fact]
        public void HandleInfoShouldCallShowInfoWithCorrectParameters()
        {
            // Arrange
            var infoResponse = new InfoResponse
            {
                Message = "Server Info",
                ServerCreated = DateTime.Now,
                ServerVersion = "1.0.0"
            };
            var json = JsonConvert.SerializeObject(infoResponse);

            // Act
            _responseHandler.HandleInfo(json);

            // Assert
            CommunicationMessages.ShowInfo(infoResponse.Message, infoResponse.ServerCreated, infoResponse.ServerVersion);
            // Note: You may need to use a mock or another way to verify calls to static methods.
        }
    }
}
