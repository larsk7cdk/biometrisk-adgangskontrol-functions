namespace biometrisk_adgangskontrol_functions.Models
{
    public class AccessRegistrationsResponse
    {
        public string Id { get; set; }
        public string EntranceStatus { get; set; }
        public string Direction { get; set; }
        public string ImageUrl { get; set; }
        public string AccessTimeStamp { get; set; }
    }
}