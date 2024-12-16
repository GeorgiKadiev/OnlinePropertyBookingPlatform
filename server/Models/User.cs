using System;
using System.Collections.Generic;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlinePropertyBookingPlatform.Utility;

namespace OnlinePropertyBookingPlatform.Models;




public partial class User
{

    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public short PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    //public string Password { get; set; } = null!;
    // Store the hashed password
    public string PasswordHash { get; set; } = string.Empty; 

    public string Role { get; set; } = null!;

    public string? ResetPasswordToken { get; set; }

    public virtual ICollection<Estate> Estates { get; set; } = new List<Estate>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public object Password { get; internal set; }

    // Method to set a hashed password
    public void SetPassword(string password)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Method to verify the password
    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }
}
