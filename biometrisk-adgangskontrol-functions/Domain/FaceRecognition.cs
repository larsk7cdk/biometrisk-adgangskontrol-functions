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
    public class FaceRecognition : IFaceRecognition
    {
        private static readonly string FACE_CONNECTION = Environment.GetEnvironmentVariable("FaceConnection");
        private static readonly string FACE_PRIMARY_KEY = Environment.GetEnvironmentVariable("FacePrimaryKey");
        private const string RECOGNITION_MODEL3 = RecognitionModel.Recognition03;

        // TODO Skal læses fra image mappen
        private const string TARGET_FACE_IDS = "E3481CCB-6A7D-4E1E-ADAC-688855D4224F";
        private static readonly IList<string> TARGET_IMAGE_FILE_NAMES = new List<string>
        {
            "LAL1.jpg"
        };


        public async Task<IList<SimilarFace>> FaceAccessControl(IFormFile file)
        {
            var client = Authenticate(FACE_CONNECTION, FACE_PRIMARY_KEY);
            return await FindSimilar(client, RECOGNITION_MODEL3, file);
        }

        public async Task<PersistedFace> FaceEntity(Guid persistedFaceId)
        {
            var client = Authenticate(FACE_CONNECTION, FACE_PRIMARY_KEY);
            return await client.LargeFaceList.GetFaceAsync(TARGET_FACE_IDS, persistedFaceId);
        }

        private static IFaceClient Authenticate(string endpoint, string key) => new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };

        private static async Task<IList<SimilarFace>> FindSimilar(IFaceClient client, string recognitionModel, IFormFile fileSrc)
        {
            IList<Guid?> targetFaceIds = new List<Guid?>();

            for (var i = 0; i < TARGET_IMAGE_FILE_NAMES.Count; i++)
            {
                var filename = "images/" + TARGET_IMAGE_FILE_NAMES[i];
                var fileByte = File.ReadAllBytes(filename);
                Stream s = new MemoryStream(fileByte);
                var faces = await DetectFaceRecognize(client, s, recognitionModel);
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