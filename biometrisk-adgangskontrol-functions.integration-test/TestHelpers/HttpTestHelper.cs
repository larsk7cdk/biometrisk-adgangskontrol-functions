using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;

namespace biometrisk_adgangskontrol_functions.integration_test.TestHelpers
{
    public static class HttpTestHelper
    {
        public static HttpRequest CreateHttpRequestWithFileAndDirection()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues> { { "direction", "enter" } }, new FormFileCollection { CreateFormFile() });

            return request;
        }

        public static IFormFile CreateFormFile()
        {
            return new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.txt");
        }
    }
}