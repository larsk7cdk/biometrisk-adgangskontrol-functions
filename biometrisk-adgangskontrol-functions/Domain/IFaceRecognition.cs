using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace biometrisk_adgangskontrol_functions.Domain
{
    public interface IFaceRecognition
    {
        Task<IList<SimilarFace>> FaceAccessControl(IFormFile file);
    }
}