namespace OnlinePropertyBookingPlatform.Models.DataTransferObjects
{
    public class EstateFilterDto
    {
        public string? Location { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsEcoFriendly { get; set; }
        public bool? IsNomadFriendly { get; set; }
        public List<string>? Amenities { get; set; }
        public int? NumberOfPersons { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
