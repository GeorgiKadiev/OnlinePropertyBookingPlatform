using System;
using System.Collections.Generic;

namespace OnlinePropertyBookingPlatform.Models;



public partial class User
{

    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public short PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? ResetPasswordToken { get; set; }

    public virtual ICollection<Estate> Estates { get; set; } = new List<Estate>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
