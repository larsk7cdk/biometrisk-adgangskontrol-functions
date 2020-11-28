using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace biometrisk_adgangskontrol_functions.Domain
{
    public interface IImageStorage
    {
        Task<string> Save(IFormFile file);
        Task<IList<byte[]>> GetAllReferenceImages();
    }
}