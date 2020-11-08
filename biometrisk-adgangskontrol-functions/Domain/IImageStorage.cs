using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace biometrisk_adgangskontrol_functions.Domain
{
    public interface IImageStorage
    {
        Task<string> Save(IList<SimilarFace> similarFaces, IFormFile file);
    }
}