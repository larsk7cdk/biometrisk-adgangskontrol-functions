using System;
using System.Linq;
using System.Threading.Tasks;
using biometrisk_adgangskontrol_functions.Core;
using biometrisk_adgangskontrol_functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace biometrisk_adgangskontrol_functions.AccessControl
{
    public static class AccessControlFunction
    {
        [FunctionName("AccessControl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // TODO Husk validering


            // Image fra request
            var file = req.Form.Files[0];

            // Undersøg om ansigt kan genkendes
            var similarFaces = await FaceRecognition.FaceAccessControl(file);

            // var faceEntity = FaceRecognition.FaceEntity((Guid) similarFaces[0].PersistedFaceId);


            // Gem på Blob storage
            await UploadPhoto.Run(similarFaces, file);

            var response = new AccessControlResponse
            {
                AccessConfirmed = similarFaces.Any() && similarFaces[0].Confidence > 0.9,
                SimilarFaces = similarFaces
            };

            return new OkObjectResult(response);
        }
    }
}