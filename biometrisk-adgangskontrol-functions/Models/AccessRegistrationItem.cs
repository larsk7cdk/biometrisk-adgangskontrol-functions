using System;

namespace biometrisk_adgangskontrol_functions.Models
{
    public class AccessRegistrationItem
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public DateTime AccessTimeStamp { get; set; }
    }
}