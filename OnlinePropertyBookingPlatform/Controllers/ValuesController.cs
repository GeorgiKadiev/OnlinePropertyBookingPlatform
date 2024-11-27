using Microsoft.AspNetCore.Mvc;
using OnlinePropertyBookingPlatform.Models;
using System;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly PropertyManagementContext _context;

        public TestController(PropertyManagementContext context)
        {
            _context = context;
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {

                var estateCount = _context.Estates.Count();
                return Ok($"Database connection successful! Total estates: {estateCount}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database connection failed: {ex.Message}");
            }
        }
    }
}
