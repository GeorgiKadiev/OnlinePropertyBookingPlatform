using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataTransferObjects;
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

        [Authorize(Roles = "Customer")]
        [HttpPost("create/{estateId}")]
        public IActionResult Create([FromBody] Review review, int estateId)
        {
            if (!_context.Estates.Any(e => e.Id == estateId)) return BadRequest("Estate doesn't exist");
            if (review == null)
            {
                return BadRequest("Invalid input.");
            }

            // Санитизация на данните
            review.Comment = _sanitizer.Sanitize(review.Comment);
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            // Добавяне на ID на автора
            var userId = userIdClaim.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }
            review.AuthorId = int.Parse(userId);
            review.EstateId = estateId;

            // Настройване на датата на ревюто
            review.Date = DateOnly.FromDateTime(DateTime.UtcNow);

            // Запазване на ревюто в базата данни
            _context.Reviews.Add(review);
            _context.SaveChanges();

            return Ok();
        }


        [Authorize(Roles = "Customer")]
        [HttpPut("edit")]
        public IActionResult Edit([FromBody] Review review)
        {
            var reviewToEdit = _context.Reviews.FirstOrDefault(r => r.Id == review.Id);
            if (reviewToEdit == null)
            {
                return BadRequest("Review doesn't exist.");
            }
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (reviewToEdit.AuthorId != int.Parse(userIdClaim.Value))
            {
                return BadRequest("cannot edit a review that is not yours");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
        [Authorize(Roles = "Customer,Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(r => r.Author)
                    .Include(r => r.Estate)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        AuthorId = r.AuthorId,
                        EstateId = r.EstateId,
                        EstateName = r.Estate.Title,
                        AuthorEmail = r.Author.Email,
                        AuthorName = r.Author.Username,
                        Rating = r.Rating,
                        Date = r.Date,
                        Comment = r.Comment
                    })
                    .ToListAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReviewDetails(int id)
        {
            try
            {
                var r = await _context.Reviews.FindAsync(id);
                if (r == null)
                {
                    return NotFound($"review with ID {id} not found.");
                }
                ReviewDto dto = new ReviewDto()
                {
                    Id = r.Id,
                    AuthorId = r.AuthorId,
                    EstateId = r.EstateId,
                    Rating = r.Rating,
                    Date = r.Date,
                    Comment = r.Comment
                };
                if(!_context.Users.Any(u=> u.Id == r.AuthorId))
                {
                    return NotFound("Review doesn't match an user");
                }
                if(!_context.Estates.Any(e=>e.Id ==r.EstateId))
                {
                    return NotFound("Review doesn't match an estate");
                }

                dto.AuthorName = _context.Users.First(u => u.Id == r.AuthorId).Username;
                dto.AuthorEmail = _context.Users.First(u => u.Id == r.AuthorId).Email;
                dto.EstateName = _context.Estates.First(e => e.Id == r.EstateId).Title;




                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize]
        [HttpGet("user-reviews/{userId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUserReviews(int userId)
        {
            try
            {
                if (!_context.Users.Any(u => u.Id == userId)) return BadRequest("User doesn't exist");
                var reviews = await _context.Reviews.Where(r=>r.AuthorId==userId)
                    .Include(r => r.Author)
                    .Include(r => r.Estate)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        AuthorId = r.AuthorId,
                        EstateId = r.EstateId,
                        EstateName = r.Estate.Title,
                        AuthorEmail = r.Author.Email,
                        AuthorName = r.Author.Username,
                        Rating = r.Rating,
                        Date = r.Date,
                        Comment = r.Comment
                    })
                    .ToListAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("estate-reviews/{estateId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllEstateReviews(int estateId)
        {
            try
            {
                if(!_context.Estates.Any(e=>e.Id == estateId))
                {
                    return BadRequest("Estate doesn't exist");
                }
                var reviews = await _context.Reviews
                    .Where(r => r.EstateId == estateId)
                    .Include(r=>r.Author)
                    .Include(r=>r.Estate)
                    .Select(r=> new ReviewDto
                    {
                        Id = r.Id,
                        AuthorId = r.AuthorId,
                        EstateId = r.EstateId,
                        EstateName = r.Estate.Title,
                        AuthorEmail = r.Author.Email,
                        AuthorName = r.Author.Username,
                        Rating = r.Rating,
                        Date = r.Date,
                        Comment = r.Comment
                    })
                    .ToListAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
