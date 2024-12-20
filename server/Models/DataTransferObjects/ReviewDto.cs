namespace OnlinePropertyBookingPlatform.Models.DataTransferObjects
{
    public class ReviewDto
    {
        public int Id { get; set; }

        public int? EstateId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateOnly? Date { get; set; }

        public int? AuthorId { get; set; }

        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string EstateName { get; set; }
    }
}
