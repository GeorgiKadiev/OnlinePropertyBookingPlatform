﻿namespace OnlinePropertyBookingPlatform.Models.DataTransferObjects
{
    public class EstateDto
    {
        public int Id { get; set; }

        public string Location { get; set; } = null!;

        public string? Title { get; set; }

        public double PricePerNight { get; set; }

        public int? EstateOwnerId { get; set; }

        public string? Description { get; set; }
        public string EstateOwnerName { get; set; }
        public List<string> Photos { get; set; }
        public List<string> Amenities { get; set; }
    }
}
