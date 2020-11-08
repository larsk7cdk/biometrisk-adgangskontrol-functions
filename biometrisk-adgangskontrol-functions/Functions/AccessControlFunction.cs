using System;
using System.Linq;
using System.Threading.Tasks;
using biometrisk_adgangskontrol_functions.Domain;
using biometrisk_adgangskontrol_functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace biometrisk_adgangskontrol_functions.Functions
{
    public static class AccessControlFunction
    {
        [FunctionName("AccessControl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequest req,
            [Queue("access-registration-queue", Connection = "QueueConnection")]
            ICollector<AccessRegistrationQueueItem> outputQueueItem,
            ILogger log)
        {
            log.LogInformation("Access Control triggered");

            // TODO Husk validering


            // Image fra request
            var file = req.Form.Files[0];

            // Undersøg om ansigt kan genkendes
            var similarFaces = await new FaceRecognition().FaceAccessControl(file);
            var accessConfirmed = similarFaces.Any() && similarFaces[0].Confidence > 0.9;


            // Gem billede på Blob storage
            var imageUrl = await new ImageStorageStorage().Save(similarFaces, file);

            // Læg registrering på kø
            var queueItem = new AccessRegistrationQueueItem
            {
                Status = accessConfirmed ? "Godkendt" : "Afvist",
                ImageUrl = imageUrl,
                AccessTimeStamp = DateTime.Now
            };

            outputQueueItem.Add(queueItem);


            // Response
            var response = new AccessControlResponse
            {
                AccessConfirmed = accessConfirmed,
                SimilarFaces = similarFaces
            };

            return new OkObjectResult(response);
        }
    }
}