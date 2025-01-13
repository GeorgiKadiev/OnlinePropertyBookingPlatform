using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OnlinePropertyBookingPlatform.Repositories;
using OnlinePropertyBookingPlatform.Utility;
using OnlinePropertyBookingPlatform.Models.DataTransferObjects;


namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly CrudRepository<Reservation> _reservationRepository;
        private readonly InputSanitizer _sanitizer;
        private readonly IEmailSender _emailSender;



        public ReservationController(PropertyManagementContext context, CrudRepository<Reservation> reservationRepository, InputSanitizer sanitizer)
        {
            _context = context;
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _sanitizer = sanitizer;

        }

        // Създаване на резервация (само клиенти)
        [Authorize(Roles = "Customer")]
        [HttpPost("create/{estateId}/{roomId}")]
        public IActionResult Create([FromBody]Reservation reservation, int estateId, int roomId)
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
            if (!_context.Rooms.Any(r => r.Id == roomId))
                return BadRequest("Room doesn't exist");
            if(!_context.Estates.Any(r => r.Id == estateId))
                return BadRequest("Estate doesn't exist");
            reservation.EstateId = estateId;
            reservation.RoomId = roomId;

            // Sanitизиране на входните данни
            reservation.CheckInDate = DateOnly.Parse(_sanitizer.Sanitize(reservation.CheckInDate.ToString()));
            reservation.CheckOutDate = DateOnly.Parse(_sanitizer.Sanitize(reservation.CheckOutDate.ToString()));
            if(reservation.CheckInDate==reservation.CheckOutDate)
            {
                reservation.CheckOutDate = reservation.CheckInDate.AddDays(1);
            }
            if(!IsRoomAvailable(roomId, reservation.CheckInDate, reservation.CheckOutDate))
            {
                return BadRequest("room is not avaivable for one of the given days");
            }
            //TimeSpan difference = reservation.CheckInDate.ToDateTime(TimeOnly.MinValue)
            //    - reservation.CheckOutDate.ToDateTime(TimeOnly.MinValue);
            //int days = difference.Days;
            //reservation.TotalPrice = days * _context.Rooms.First(r=>r.Id == roomId).


            _context.Reservations.Add(reservation);
            _context.SaveChanges();


            // Email notification
            var user = _context.Users.FirstOrDefault(u => u.Id == userId); // Retrieve user details
            if (user != null)
            {
                _emailSender.SendEmailAsync(
                    email: user.Email,
                    subject: "Reservation Confirmation",
                    message: $"Dear {user.Username},<br><br>Your reservation for Estate ID {estateId} has been confirmed.<br>Check-in: {reservation.CheckInDate}<br>Check-out: {reservation.CheckOutDate}.<br><br>Thank you for booking with us!"
                );
            }


            return Ok();
            

        }

        // Изтриване на резервация 
        [Authorize]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            //добавено Панчо
            var reservation = _context.Reservations
        .Include(r => r.Customer) 
        .FirstOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return BadRequest("Reservation doesn't exist.");
            }

            var user = reservation.Customer;

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();

            // Send email notification if the user exists
            if (user != null)
            {
                _emailSender.SendEmailAsync(
                    email: user.Email,
                    subject: "Reservation Canceled",
                    message: $"Dear {user.Username},<br><br>Your reservation for Estate ID {reservation.EstateId} has been canceled.<br><br>If you have any questions, please contact support."
                );
            }




            return Ok(); 
        }

        // Извличане на всички резервации (само администратори)
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-reservations")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAllReservations()
        {

            try
            {
                var reservations = await _context.Reservations
                    .Include(r => r.Customer)   // Include Customer (User)
                     .Include(r => r.Estate)
                     .Select(r => new ReservationDto
                     {
                         Id = r.Id,
                         CustomerId = r.CustomerId,
                         EstateId = r.EstateId,
                         CheckInDate = r.CheckInDate,
                         CheckOutDate = r.CheckOutDate,
                         TotalPrice = r.TotalPrice,
                         Status = r.Status,
                         CustomerName = r.Customer.Username,
                         EstateName = r.Estate.Title
                     })
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
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAllReservationsFromAnUser(int userId)
        {
            try
            {
                var reservations = await _context.Reservations
                    .Where(r=>r.CustomerId==userId)
                    .Include(r => r.Customer)   // Include Customer (User)
                     .Include(r => r.Estate)
                    .Select(r => new ReservationDto
                    {
                        Id = r.Id,
                        CustomerId = r.CustomerId,
                        EstateId = r.EstateId,
                        CheckInDate = r.CheckInDate,
                        CheckOutDate = r.CheckOutDate,
                        TotalPrice = r.TotalPrice,
                        Status = r.Status,
                        CustomerName = r.Customer.Username,
                        EstateName = r.Estate.Title
                    })
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
        public async Task<ActionResult<ReservationDto>> GetReservationDetails(int id)
        {
            try
            {
                var r = await _context.Reservations.FindAsync(id);
                if (r == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }
                var dto = new ReservationDto()
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    EstateId = r.EstateId,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status,
                };
                dto.CustomerName = _context.Users
                    .FirstOrDefault(u => u.Id == r.CustomerId)
                    .Username;
                dto.EstateName = _context.Estates
                    .FirstOrDefault(e => e.Id == r.EstateId)
                    .Title;



                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public bool IsRoomAvailable(int roomId, DateOnly checkInDate, DateOnly checkOutDate)
        {

            var reservations = _context.Reservations
       .Where(r => r.RoomId == roomId)
       .ToList(); // Materialize the query to a list.

            // Generate all dates for the new reservation
            var newReservationDates = Enumerable.Range(0, (checkOutDate.ToDateTime(TimeOnly.MinValue) - checkInDate.ToDateTime(TimeOnly.MinValue)).Days)
                                                .Select(offset => checkInDate.AddDays(offset))
                                                .ToHashSet(); // Use HashSet for efficient lookup.

            // Check for overlap
            foreach (var reservation in reservations)
            {
                var occupiedDates = Enumerable.Range(0, (reservation.CheckOutDate.ToDateTime(TimeOnly.MinValue) - reservation.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days)
                                              .Select(offset => reservation.CheckInDate.AddDays(offset));

                if (occupiedDates.Any(date => newReservationDates.Contains(date)))
                {
                    return false; // Conflict found.
                }
            }

            return true; // No conflicts.
        }



    }
}
