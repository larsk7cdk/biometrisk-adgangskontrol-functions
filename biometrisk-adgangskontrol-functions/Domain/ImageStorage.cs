using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace biometrisk_adgangskontrol_functions.Domain
{
    /// <summary>
    ///     Konvertering af FormFile til jpeg billede og gem på Blob storage
    /// </summary>
    public class ImageStorage : IImageStorage
    {
        private static readonly string STORAGE_CONNECTION = Environment.GetEnvironmentVariable("StorageConnection");
        private static readonly string BLOB_STORAGE_IMAGE_URL = Environment.GetEnvironmentVariable("BlobStorageImageUrl");
        private const string CONTAINER_NAME_SAVE = "access-registration-images";
        private const string CONTAINER_NAME_REFERENCE = "reference-images";

        /// <summary>
        ///     Modtager en FormFile og konverterer til jpeg billede og gemmer på Blob Storage
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Image url i Blob Storage</returns>
        public async Task<string> Save(IFormFile file)
        {
            var container = new BlobContainerClient(STORAGE_CONNECTION, CONTAINER_NAME_SAVE);

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

            return $"{BLOB_STORAGE_IMAGE_URL}{file.FileName}";
        }

        /// <summary>
        ///     Henter reference billeder på Blob Storage
        /// </summary>
        /// <returns>Reference billeder som en liste af byte array</returns>
        public async Task<IList<byte[]>> GetAllReferenceImages()
        {
            var container = new BlobContainerClient(STORAGE_CONNECTION, CONTAINER_NAME_REFERENCE);

            IList<byte[]> images = new List<byte[]>();

            await foreach (var blobItem in container.GetBlobsAsync())
            {
                await using var ms = new MemoryStream();
                await container.GetBlobClient(blobItem.Name).DownloadToAsync(ms);
                images.Add(ms.ToArray());
            }

            return images;
        }
    }
}