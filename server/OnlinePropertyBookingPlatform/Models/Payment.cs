using System;
using System.Collections.Generic;

namespace OnlinePropertyBookingPlatform.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? ReservationId { get; set; }

    public double Amount { get; set; }

    public string? Method { get; set; }

    public DateOnly? Date { get; set; }

    public int? Status { get; set; }

    public virtual Reservation? Reservation { get; set; }
}
