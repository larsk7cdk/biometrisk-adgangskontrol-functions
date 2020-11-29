using biometrisk_adgangskontrol_functions.Models;
using biometrisk_adgangskontrol_functions.unit_test.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace biometrisk_adgangskontrol_functions.unit_test.AccessControl
{
    public class AccessControlFunctionTest
    {
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly Mock<ICollector<AccessRegistrationQueueItem>> _queueMock = new Mock<ICollector<AccessRegistrationQueueItem>>();
        
        [Fact]
        public void Http_trigger_should_return_badrequest_when_no_formfile()
        {
            // Arrange
            var expected = 400;

            // Act
            var actual =
                Functions.AccessControlFunction.Run(
                    HttpTestHelper.CreateHttpRequest(),
                    _queueMock.Object,
                    _loggerMock.Object)
                    .Result as BadRequestObjectResult;

            // Assert
            Assert.Equal(expected, actual.StatusCode);
        }

        [Fact]
        public void Http_trigger_should_return_badrequest_when_no_direction()
        {
            // Arrange
            var expected = 400;

            // Act
            var actual =
                Functions.AccessControlFunction.Run(
                    HttpTestHelper.CreateHttpRequestWithFile(),
                    _queueMock.Object,
                    _loggerMock.Object)
                    .Result as BadRequestObjectResult;

            // Assert
            Assert.Equal(expected, actual.StatusCode);
        }
    }
}