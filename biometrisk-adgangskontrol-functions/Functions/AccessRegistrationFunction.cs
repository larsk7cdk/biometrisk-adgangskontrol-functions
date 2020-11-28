using System;
using System.Collections.Generic;
using System.Linq;
using biometrisk_adgangskontrol_functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace biometrisk_adgangskontrol_functions.Functions
{
    public static class AccessRegistrationFunction
    {
        /// <summary>
        ///     Læs adgangsregistrering fra kø og gem i Cosmos DB
        /// </summary>
        [FunctionName("AccessRegistrationQueueFunction")]
        public static void Run(
            [QueueTrigger("access-registration-queue", Connection = "QueueConnection")]
            AccessRegistrationQueueItem queueItem,
            [CosmosDB("slbioakdatabase", "access-registration-items", ConnectionStringSetting = "CosmosDBConnection")]
            out AccessRegistrationItem item,
            ILogger log)
        {
            log.LogInformation("Læs adgangsregistrering fra kø og gem i Cosmos DB");

            item = new AccessRegistrationItem
            {
                Id = Guid.NewGuid().ToString(),
                EntranceStatus = queueItem.EntranceStatus,
                Direction = queueItem.Direction,
                ImageUrl = queueItem.ImageUrl,
                AccessTimeStamp = queueItem.AccessTimeStamp
            };
        }

        /// <summary>
        ///     Hent adgangsregistreringer fra Cosmos DB
        /// </summary>
        [FunctionName("AccessRegistrationsFunction")]
        public static IActionResult Registrations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registrations")]
            HttpRequest req,
            [CosmosDB("slbioakdatabase", "access-registration-items", ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM items ORDER BY items.AccessTimeStamp DESC")]
            IEnumerable<AccessRegistrationItem> accessRegistrations,
            ILogger log)
        {
            log.LogInformation("Hent adgangsregistreringer fra Cosmos DB");

            var response = accessRegistrations.Select(s =>
                new AccessRegistrationsResponse
                {
                    Id = s.Id,
                    EntranceStatus = s.EntranceStatus,
                    Direction = s.Direction,
                    ImageUrl = s.ImageUrl,
                    AccessTimeStamp = s.AccessTimeStamp.ToString("yyyy-MM-dd HH:mm")
                }
            );

            return new OkObjectResult(response);
        }
    }
}