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
    /// <summary>
    ///     Billede og adgangsretning fra ansigt app modtages og gemmes på kø
    /// </summary>
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
            string direction;

            log.LogInformation("Adgang registreret");

            // Validering
            if (!req.Form.Files.Any())
            {
                const string error = "Der er ikke vedhæftet en fil";
                log.LogError(error);
                return new BadRequestObjectResult(error);
            }

            if (req.Form.TryGetValue("direction", out var directionValues))
            {
                direction = directionValues[0].ToLowerInvariant() == "enter" ? "Kommet" : "Gået";
                log.LogInformation($"Adgangsretning: {direction}");
            }
            else
            {
                const string error = "Der er ikke information om adgangsretning";
                log.LogError(error);
                return new BadRequestObjectResult(error);
            }

            // Image fra request
            var file = req.Form.Files[0];

            // Undersøg om ansigt kan genkendes
            log.LogInformation("Undersøg om ansigt kan genkendes");
            var similarFaces = await new FaceRecognition().FaceAccessControl(file);
            var accessConfirmed = similarFaces.Any() && similarFaces[0].Confidence > 0.9;
            log.LogInformation(string.Format("Ansigt er {0}genkendt", accessConfirmed ? "" : "ikke "));


            // Gem billede på Blob storage
            log.LogInformation("Gem billede på Blob storage");
            var imageUrl = await new ImageStorage().Save(file);
            log.LogInformation("Billedet er gemt på Blob storage");

            // Læg registrering på kø
            log.LogInformation("Læg registrering på kø");
            var queueItem = new AccessRegistrationQueueItem
            {
                EntranceStatus = accessConfirmed ? "Godkendt" : "Afvist",
                Direction = direction,
                ImageUrl = imageUrl,
                AccessTimeStamp = DateTime.Now.ToUniversalTime().AddHours(1)
            };

            outputQueueItem.Add(queueItem);
            log.LogInformation("Registrering er på kø");


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