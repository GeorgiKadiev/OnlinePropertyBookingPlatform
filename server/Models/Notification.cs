using System;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; } // The recipient of the notification
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    public virtual User User { get; set; }
}
