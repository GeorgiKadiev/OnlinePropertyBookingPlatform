using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Repositories;


namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly SecureRepository _secureRepository;


        public ReviewController(PropertyManagementContext context, SecureRepository secureRepository)
        {
            _context = context;
            _secureRepository = secureRepository;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Review review)
        {
            var userId = User.FindFirst("UserId")?.Value;
            review.AuthorId = int.Parse(userId);

            await _secureRepository.AddEntityAsync(review);
            return Ok();
            /*
            //трябва да се добави Id-то на потребителят, към ревюто
            await _secureRepository.AddEntityAsync(review);
            //_context.Reviews.Add(review);
            //_context.SaveChanges();
            return Ok();
            */

        }
        [Authorize(Roles = "Customer,Admin")]
        [HttpPost("edit")]
        public IActionResult Edit([FromBody] Review review)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Reviews.Any(r => r.Id == review.Id))
            {
                return BadRequest("review doesn't exist");
            }
            var reviewToUpdate = _context.Reviews.First(r => r.Id == review.Id);
            reviewToUpdate.Comment = review.Comment;
            reviewToUpdate.Rating = review.Rating;

            _context.Update(reviewToUpdate);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return BadRequest("Review doesn't exist");

            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return Ok();
            /*
            if (!_context.Reviews.Any(r => r.Id == id))
            {
                return BadRequest("review doesn't exist");
            }
            Review review = _context.Reviews.Where(r => r.Id == id).First();
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return Ok();
            */
        }

        [Authorize(Roles = "Admin")]
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
