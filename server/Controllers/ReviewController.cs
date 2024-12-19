using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Utility;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly InputSanitizer _sanitizer;


        public ReviewController(PropertyManagementContext context, InputSanitizer sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Review review)
        {
            if (review == null)
            {
                return BadRequest("Invalid input.");
            }

            // Санитизация на данните
            review.Comment = _sanitizer.Sanitize(review.Comment);

            // Добавяне на ID на автора
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }
            review.AuthorId = int.Parse(userId);

            // Настройване на датата на ревюто
            review.Date = DateOnly.FromDateTime(DateTime.UtcNow);

            // Запазване на ревюто в базата данни
            _context.Reviews.Add(review);
            _context.SaveChanges();

            return Ok();
        }


        [HttpPost("edit")]
        public IActionResult Edit([FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewToEdit = _context.Reviews.FirstOrDefault(r => r.Id == review.Id);
            if (reviewToEdit == null)
            {
                return BadRequest("Review doesn't exist.");
            }

            // Санитизация на входните данни
            reviewToEdit.Comment = _sanitizer.Sanitize(review.Comment);
            reviewToEdit.Rating = review.Rating;

            _context.Update(reviewToEdit);
            _context.SaveChanges();

            return Ok();

  /*
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (!_context.Reviews.Any(r => r.Id == review.Id))
            {
                return BadRequest("review doesn't exist");
            }

            Review review1 = _context.Reviews.Where(r => r.Id == review.Id).First();
            _context.Update(review1);
            _context.SaveChanges();

            return Ok();
            */
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_context.Reviews.Any(r => r.Id == id))
            {
                return BadRequest("review doesn't exist");
            }
            Review review = _context.Reviews.Where(r => r.Id == id).First();
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllReviews()
        {
            try
            {
                var users = await _context.Reviews.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Estate>> GetReviewDetails(int id)
        {
            try
            {
                var estate = await _context.Reviews.FindAsync(id);

                if (estate == null)
                {
                    return NotFound($"review with ID {id} not found.");
                }

                return Ok(estate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("user-reviews/{userId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUserReviews(int userId)
        {
            try
            {
                var users = await _context.Reviews.Where(r=>r.AuthorId==userId).ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
