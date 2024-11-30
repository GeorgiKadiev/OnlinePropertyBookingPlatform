using System;
using System.Collections.Generic;

namespace OnlinePropertyBookingPlatform.Models;

public partial class Room
{
    public int Id { get; set; }

    public int? EstateId { get; set; }

    public string? RoomType { get; set; }

    public int? BedCount { get; set; }

    public int? MaxGuests { get; set; }

    public virtual Estate? Estate { get; set; }
}
