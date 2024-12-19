using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Utility;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly InputSanitizer _sanitizer;


        public RoomController(PropertyManagementContext context,InputSanitizer sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }
        [HttpPost("{estateId}")]
        public IActionResult Create([FromBody] Room room, int estateId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Санитизация на входните данни
            room.EstateId = estateId;
            room.Name = _sanitizer.Sanitize(room.Name);  // Санитизация на името на стаята
            room.Description = _sanitizer.Sanitize(room.Description); // Санитизация на описанието

            _context.Rooms.Add(room);
            _context.SaveChanges();
            return Ok();

        }
        [HttpPost("edit")]
        public IActionResult Edit([FromBody] Room room)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roomToEdit = _context.Rooms.FirstOrDefault(r => r.Id == room.Id);

            if (!_context.Rooms.Any(r => r.Id == room.Id))
            {
                return BadRequest("room doesn't exist");
            }
            // Санитизация на входните данни
            roomToEdit.Name = _sanitizer.Sanitize(room.Name); // Санитизация на името на стаята
            roomToEdit.Description = _sanitizer.Sanitize(room.Description); // Санитизация на описанието

            Room room1 = _context.Rooms.Where(r => r.Id == room.Id).First();

            _context.Update(room1);
            _context.SaveChanges();

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_context.Rooms.Any(r => r.Id == id))
            {
                return BadRequest("room doesn't exist");
            }
            Room room = _context.Rooms.Where(r => r.Id == id).First();
            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("{estateId}")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRoomsForAnEstate(int estateId)
        {
            try
            {
                var rooms = await _context.Rooms.Where(r=>r.EstateId==estateId).ToListAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("details/{roomId}")]
        public async Task<ActionResult<Estate>> GetRoomDetails(int roomId)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(roomId);

                if (room == null)
                {
                    return NotFound($"room with ID {roomId} not found.");
                }

                return Ok(roomId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
