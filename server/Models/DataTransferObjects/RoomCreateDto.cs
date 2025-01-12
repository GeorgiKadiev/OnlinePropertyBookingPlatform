public class RoomCreateDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int MaxGuests { get; set; }
    public int BedCount { get; set; }
    public string RoomType { get; set; } = null!;
    public int EstateId { get; set; } // Link to an estate
    public List<string> Amenities { get; set; } = new List<string>(); // List of selected amenity names
}
