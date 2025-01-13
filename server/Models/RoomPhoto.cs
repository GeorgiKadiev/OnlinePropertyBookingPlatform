namespace OnlinePropertyBookingPlatform.Models
{
    public class RoomPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int RoomId { get; set; }
        public Room room { get; set; }
    }
}
