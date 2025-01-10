using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _service;

    public NotificationController(NotificationService service)
    {
        _service = service;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateNotification(int userId, string title, string message)
    {
        await _service.CreateNotification(userId, title, message);
        return Ok("Notification created successfully");
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId)
    {
        var notifications = await _service.GetUserNotifications(userId);
        return Ok(notifications);
    }

    [HttpPost("mark-as-read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _service.MarkAsRead(id);
        return Ok("Notification marked as read");
    }
}
