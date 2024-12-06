using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : Controller
    {
        private readonly PropertyManagementContext _context;

        public ReservationController(PropertyManagementContext context)
        {
            _context = context;
        }

        [HttpPost("{estateId}")]
        public IActionResult Create(Reservation reservation, int estateId)
        {
            reservation.EstateId = estateId;
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            return Ok();
            

        }
        [HttpPost("edit")]
        public IActionResult Edit(Reservation reservation)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Reservations.Any(r => r.Id == reservation.Id))
            {
                return BadRequest("Reservation doesn't exist");
            }
            Reservation reservation1 = _context.Reservations.Where(r => r.Id == reservation.Id).First();
            _context.Update(reservation1);
            _context.SaveChanges();

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_context.Reservations.Any(r => r.Id == id))
            {
                return BadRequest("Reservation doesn't exist");
            }
            Reservation reservation = _context.Reservations.Where(r => r.Id == id).First();
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
            return Ok();
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllReservations()
        {
            try
            {
                var users = await _context.Reservations.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Estate>> GetReservationDetails(int id)
        {
            try
            {
                var estate = await _context.Reservations.FindAsync(id);

                if (estate == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }

                return Ok(estate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
