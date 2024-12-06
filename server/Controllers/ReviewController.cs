using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly PropertyManagementContext _context;

        public ReviewController(PropertyManagementContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create(Review review)
        {
            //трябва да се добави Id-то на потребителят, към ревюто
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return Ok();


        }
        [HttpPost("edit")]
        public IActionResult Edit(Review review)
        {

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
