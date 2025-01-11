namespace OnlinePropertyBookingPlatform.Models.DataTransferObjects
{
    public class RoomDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int EstateId { get; set; }

        public string? RoomType { get; set; }

        public int? BedCount { get; set; }

        public int? MaxGuests { get; set; }

        public string EstateName { get; set; }
        public List<DateOnly> DatesWhenOccupied { get; set; }
        public List<string> Photos { get; set; }
    }
}
