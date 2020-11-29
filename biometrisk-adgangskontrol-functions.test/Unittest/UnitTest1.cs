using System.Collections.Generic;
using System.IO;
using System.Text;
using biometrisk_adgangskontrol_functions.Functions;
using biometrisk_adgangskontrol_functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace biometrisk_adgangskontrol_functions.tests
{
    public class UnitTest1
    {
        private readonly Mock<ILogger> loggerMock = new Mock<ILogger>();


        private readonly Mock<ICollector<AccessRegistrationQueueItem>> queueMock =
            new Mock<ICollector<AccessRegistrationQueueItem>>();

        public static HttpRequest CreateHttpRequest()
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Form = new FormCollection(new Dictionary<string, StringValues>());
            return request;
        }

        public static HttpRequest CreateHttpRequestWithFile()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;

            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                "dummy.txt");
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection
            {
                file
            });

            return request;
        }

        [Fact]
        public async void Test1()
        {
            // Arrange


              // Act
              var result =
                AccessControlFunction.Run(CreateHttpRequestWithFile(), queueMock.Object, loggerMock.Object).Result
                    ;

            // Assert
            // Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async void Test2()
        {
            // Arrange

            // Act
            var result =
                AccessControlFunction.Run(CreateHttpRequest(), queueMock.Object, loggerMock.Object).Result as
                    BadRequestObjectResult;

            // Assert
            Assert.Equal(400, result.StatusCode);
        }
    }
}