using System;

namespace biometrisk_adgangskontrol_functions.Models
{
    public class AccessRegistrationQueueItem
    {
        public string EntranceStatus { get; set; }
        public string Direction { get; set; }
        public string ImageUrl { get; set; }
        public DateTime AccessTimeStamp { get; set; }
    }
}