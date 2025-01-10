using System.Threading.Tasks;

public class NotificationService
{
    private readonly PropertyManagementContext _context;

    public NotificationService(PropertyManagementContext context)
    {
        _context = context;
    }

    public async Task CreateNotification(int userId, string title, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetUserNotifications(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    public async Task MarkAsRead(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}
