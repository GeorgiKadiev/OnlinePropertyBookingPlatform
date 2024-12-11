﻿using Microsoft.AspNetCore.Mvc;
using OnlinePropertyBookingPlatform.Models;
using System;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly PropertyManagementContext _context;
        private readonly Utility.IEmailSender _sender;

        public TestController(PropertyManagementContext context, Utility.IEmailSender sender)
        {
            _context = context;
            _sender = sender;
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

        //направено с цел тестване на изпращането на мейл
        [HttpGet("email")]
        public IActionResult EmailSend()
        {
            try
            {

                _sender.SendEmailAsync("hutchyy@abv.bg", "asd", "asd");
                return Ok("email sent");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sendin email failed: {ex.Message}");
            }
        }
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
        [HttpGet("user")]
        public async Task<ActionResult<string>> GetUserId()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == string.Empty)
                    return BadRequest("no user");
                return Ok(userId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sendin email failed: {ex.Message}");
            }
        }
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
    }
}
