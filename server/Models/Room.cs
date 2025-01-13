using System;
using System.Collections.Generic;

namespace OnlinePropertyBookingPlatform.Models;

public partial class Room
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int EstateId { get; set; }

    public string? RoomType { get; set; }

    public int? BedCount { get; set; }

    public int? MaxGuests { get; set; }

    public virtual Estate? Estate { get; set; }

    // new relationship for amenities
    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
    public virtual ICollection<RoomPhoto> Photos { get; set; } = new List<RoomPhoto>();
}
