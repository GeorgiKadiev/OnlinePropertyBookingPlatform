using System;
using System.Collections.Generic;

namespace OnlinePropertyBookingPlatform.Models;

public partial class Review
{
    public int Id { get; set; }

    public int? EstateId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }
    public bool flagged { get; set; } = false;

    public DateOnly? Date { get; set; }

    public int? AuthorId { get; set; }

    public virtual User? Author { get; set; }

    public virtual Estate? Estate { get; set; }

}
