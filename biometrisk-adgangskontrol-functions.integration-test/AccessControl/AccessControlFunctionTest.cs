using biometrisk_adgangskontrol_functions.Domain;
using biometrisk_adgangskontrol_functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;

namespace biometrisk_adgangskontrol_functions.integration_test.AccessControl
{
    public class AccessControlFunctionTest
    {
        private readonly Mock<AccessRegistrationQueueItem> _accessRegistrationQueueItemMock =
            new Mock<AccessRegistrationQueueItem>();

        private readonly Mock<FaceRecognition> _faceRecognitionMock = new Mock<FaceRecognition>();
        private readonly Mock<ImageStorage> _imageStorageMock = new Mock<ImageStorage>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        private readonly Mock<ICollector<AccessRegistrationQueueItem>> _queueMock =
            new Mock<ICollector<AccessRegistrationQueueItem>>();


        //[Fact]
        //public async void Http_trigger_should_return_ok()
        //{
        //    // Arrange
        //    var similarFaceTestDouble = new[] { new SimilarFace(0.9) };
        //    var expected = 200;


        //    _faceRecognitionMock.Setup(s => s.FaceAccessControl(TestHelpers.HttpTestHelper.CreateFormFile())).ReturnsAsync(similarFaceTestDouble);

        //    // Act
        //    var actual =
        //        Functions.AccessControlFunction.Run(
        //            HttpTestHelper.CreateHttpRequestWithFileAndDirection(),
        //            _queueMock.Object,
        //            _loggerMock.Object)
        //            .Result as BadRequestObjectResult;

        //    // Assert
        //    Assert.Equal(expected, actual.StatusCode);
        //}
    }
}