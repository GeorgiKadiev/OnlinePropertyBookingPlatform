using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : Controller
{
    [HttpGet("rate-limit-test")]
    public IActionResult RateLimitTest()
    {
        return Ok("This endpoint is rate-limited. Try refreshing quickly to see it in action.");
    }
}
