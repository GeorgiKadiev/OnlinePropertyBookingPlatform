using System;
using System.Collections.Generic;

namespace OnlinePropertyBookingPlatform.Models;

public partial class Estate
{
    public int Id { get; set; }

    public string Location { get; set; } = null!;

    public string? Title { get; set; }

    public double PricePerNight { get; set; }

    public int? EstateOwnerId { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();

    public virtual User? EstateOwner { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
