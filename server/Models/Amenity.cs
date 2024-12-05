using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlinePropertyBookingPlatform.Models;

public partial class Amenity
{
    
    public int EstateId { get; set; }

    public string AmenityName { get; set; } = null!;

    public virtual Estate Estate { get; set; } = null!;
}
