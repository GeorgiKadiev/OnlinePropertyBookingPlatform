using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : Controller
    {
        private readonly PropertyManagementContext _context;

        public RoomController(PropertyManagementContext context)
        {
            _context = context;
        }
        [HttpPost("{estateId}")]
        public IActionResult Create(Room room, int estateId)
        {
            room.EstateId = estateId;
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return Ok();

        }
        [HttpPost("edit")]
        public IActionResult Edit(Room room)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Rooms.Any(r => r.Id == room.Id))
            {
                return BadRequest("room doesn't exist");
            }
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
        [HttpGet("{roomId}")]
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
