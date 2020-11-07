using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace biometrisk_adgangskontrol_functions.AccessRegistration
{
    public static class AccessRegistrationFunction
    {
        [FunctionName("BlobTriggerFunction")]
        public static void Run([BlobTrigger("photos/{name}", Connection = "StorageConnectionAppSetting")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
