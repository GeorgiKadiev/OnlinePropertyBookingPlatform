using System;
using System.Collections.Generic;

namespace OnlinePropertyBookingPlatform.Models;

public partial class Reservation
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int? EstateId { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public double? TotalPrice { get; set; }

    public bool? Status { get; set; }

    public virtual User? Customer { get; set; }

    public virtual Estate? Estate { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public int RoomId { get; set; }
    public Room? room { get; set; }
}
