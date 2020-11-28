using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace biometrisk_adgangskontrol_functions.Domain
{
    /// <summary>
    ///     Azure Face Recognition
    ///     Facade mod Azure Face API
    /// </summary>
    public class FaceRecognition : IFaceRecognition
    {
        private static readonly string FACE_CONNECTION = Environment.GetEnvironmentVariable("FaceConnection");
        private static readonly string FACE_PRIMARY_KEY = Environment.GetEnvironmentVariable("FacePrimaryKey");
        private const string RECOGNITION_MODEL3 = RecognitionModel.Recognition03;

        /// <summary>
        ///     Modtager et billede og sammenligner dette med billeder i systemet for at undersøge om der er et match
        /// </summary>
        public async Task<IList<SimilarFace>> FaceAccessControl(IFormFile file)
        {
            var client = Authenticate(FACE_CONNECTION, FACE_PRIMARY_KEY);
            return await FindSimilar(client, RECOGNITION_MODEL3, file);
        }

        private static IFaceClient Authenticate(string endpoint, string key) => new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };

        private static async Task<IList<SimilarFace>> FindSimilar(IFaceClient client, string recognitionModel, IFormFile fileSrc)
        {
            IList<Guid?> targetFaceIds = new List<Guid?>();

            var referenceImages = await new ImageStorage().GetAllReferenceImages();

            foreach (var referenceImage in referenceImages)
            {
                await using var ms = new MemoryStream(referenceImage);
                var faces = await DetectFaceRecognize(client, ms, recognitionModel);
                targetFaceIds.Add(faces[0].FaceId.Value);
            }

            using var srFile = new StreamReader(fileSrc.OpenReadStream());
            var imageBytes = Convert.FromBase64String(await srFile.ReadToEndAsync());

            await using var msImage = new MemoryStream(imageBytes, 0, imageBytes.Length);
            var image = Image.FromStream(msImage);

            await using var msSave = new MemoryStream();
            image.Save(msSave, ImageFormat.Jpeg);
            msSave.Seek(0, 0);

            var detectedFaces = await DetectFaceRecognize(client, msSave, recognitionModel);
            var similarResults = await client.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds);

            return similarResults;
        }

        private static async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, Stream file,
            string recognitionModel)
        {
            var detectedFaces = await faceClient.Face.DetectWithStreamAsync(
                file,
                recognitionModel: recognitionModel,
                detectionModel: DetectionModel.Detection02);

            return detectedFaces.ToList();
        }
    }
}