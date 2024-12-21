namespace OnlinePropertyBookingPlatform.Models.DataTransferObjects
{
    public class ReservationDto
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }

        public int? EstateId { get; set; }

        public DateOnly? CheckInDate { get; set; }

        public DateOnly? CheckOutDate { get; set; }

        public double? TotalPrice { get; set; }

        public bool? Status { get; set; }
        public string CustomerName { get; set; }
        public string EstateName { get; set; }
    }
}
