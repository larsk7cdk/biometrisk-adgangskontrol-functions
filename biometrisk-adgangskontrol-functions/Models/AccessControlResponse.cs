using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace biometrisk_adgangskontrol_functions.Models
{
    public class AccessControlResponse
    {
        public bool AccessConfirmed { get; set; }
        public IList<SimilarFace> SimilarFaces { get; set; }
    }
}