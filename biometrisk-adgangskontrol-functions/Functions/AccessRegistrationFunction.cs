using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        /// </summary>
        [FunctionName("AccessRegistrationQueueFunction")]
        public static void Run(
            [QueueTrigger("access-registration-queue", Connection = "QueueConnection")] AccessRegistrationQueueItem queueItem,
            [CosmosDB("slbioakdatabase", "access-registration-items", ConnectionStringSetting = "CosmosDBConnection")] out AccessRegistrationItem item,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueItem}");

            item = new AccessRegistrationItem
            {
                Id = Guid.NewGuid().ToString(),
                Status = queueItem.Status,
                ImageUrl = queueItem.ImageUrl,
                AccessTimeStamp = queueItem.AccessTimeStamp
            };
        }

        /// <summary>
        /// </summary>
        [FunctionName("AccessRegistrationsFunction")]
        public static IActionResult Registrations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registrations")]
            HttpRequest req,
            [CosmosDB("slbioakdatabase", "access-registration-items", ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM items ORDER BY items.AccessTimeStamp DESC")] IEnumerable<AccessRegistrationItem> accessRegistrations,
            ILogger log)
        {
            var response = accessRegistrations.Select(s =>
                new AccessRegistrationsResponse
                {
                    Id = s.Id,
                    Status = s.Status,
                    ImageUrl = s.ImageUrl,
                    AccessTimeStamp = s.AccessTimeStamp.ToString("yyyy-MM-dd HH:mm")
                }
            );

            return new OkObjectResult(response);
        }
    }
}