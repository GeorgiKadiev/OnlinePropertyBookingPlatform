using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OnlinePropertyBookingPlatform.Repositories;
using OnlinePropertyBookingPlatform.Utility;


namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly CrudRepository<Reservation> _reservationRepository;
        private readonly InputSanitizer _sanitizer;



        public ReservationController(PropertyManagementContext context, CrudRepository<Reservation> reservationRepository, InputSanitizer sanitizer)
        {
            _context = context;
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _sanitizer = sanitizer;

        }

        // Създаване на резервация (само клиенти)
        [Authorize(Roles = "Customer")]
        [HttpPost("create/{estateId}")]
        public IActionResult Create([FromBody]Reservation reservation, int estateId)
        {
            //тук трябва да се добави и Id-то на потребителят,
            //който създава резервацията
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = int.Parse(userIdClaim.Value);// ID на текущия потребител
            if (userId == null)
            {
                return Unauthorized("User ID is empty");
            }
            if(!_context.Users.Any(u=>u.Id==userId))
            {
                return BadRequest("User not found");
            }

            reservation.CustomerId = int.Parse(_sanitizer.Sanitize(userIdClaim.Value));
            // reservation.CustomerId = int.Parse(userId);
            reservation.EstateId = estateId;

            // Sanitизиране на входните данни
            reservation.CheckInDate = DateOnly.Parse(_sanitizer.Sanitize(reservation.CheckInDate.ToString()));
            reservation.CheckOutDate = DateOnly.Parse(_sanitizer.Sanitize(reservation.CheckOutDate.ToString()));

            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            return Ok();
            

        }

        // Изтриване на резервация 
        [Authorize(Roles = "Admin,Customer")]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            //добавено Панчо
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return BadRequest("Reservation doesn't exist.");
            }

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
            return Ok(); 
        }

        // Извличане на всички резервации (само администратори)
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-reservations")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllReservations()
        {

            try
            {
                var reservations = await _context.Reservations
                    .Include(r => r.Customer)   // Include Customer (User)
                     .Include(r => r.Estate)
                     .ToListAsync();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        // Извличане на резервации на потребител (само клиенти и администратори)
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("user-reservations/{userId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllReservationsFromAnUser(int userId)
        {
            try
            {
                var reservations = await _context.Reservations
                    .Where(r=>r.CustomerId==userId)
                    .Include(r => r.Customer)   // Include Customer (User)
                     .Include(r => r.Estate)
                     .ToListAsync();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Детайли за резервация (само клиенти и администратори)
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<Estate>> GetReservationDetails(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);

                if (reservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        


    }
}
