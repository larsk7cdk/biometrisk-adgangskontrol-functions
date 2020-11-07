using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace biometrisk_adgangskontrol_functions.Core
{
    public static class UploadPhoto
    {
        private static readonly string ConnectionString =
            Environment.GetEnvironmentVariable("StorageConnectionAppSetting");

        private static readonly string containerName = "photos";

        public static async Task Run(IList<SimilarFace> similarFaces, IFormFile file)
        {
            var container = new BlobContainerClient(ConnectionString, containerName);

            using var srFile = new StreamReader(file.OpenReadStream());
            var imageBytes = Convert.FromBase64String(await srFile.ReadToEndAsync());

            await using var msImage = new MemoryStream(imageBytes, 0, imageBytes.Length);
            var image = Image.FromStream(msImage);

            await using var msSave = new MemoryStream();
            image.Save(msSave, ImageFormat.Jpeg);
            msSave.Seek(0, 0);


            var blob = container.GetBlobClient(file.FileName);
            await blob.UploadAsync(msSave, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });
        }
    }
}