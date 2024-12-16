using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Repositories;
using OnlinePropertyBookingPlatform.Utility;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly SecureRepository _secureRepository;
        private readonly ILogger<UserController> _logger;



        public RoomController(PropertyManagementContext context, SecureRepository secureRepository, ILogger<UserController> logger)
        {
            _context = context;
            _secureRepository = secureRepository;
            _logger = logger;
        }

        [Authorize(Roles = "EstateOwner")]
        [ValidateAntiForgeryToken] // CSRF защита
        [HttpPost("{estateId}")]
        public async Task<IActionResult> Create([FromBody] Room room, int estateId)
        {
            // Валидация на RoomType
            if (string.IsNullOrEmpty(room.RoomType))
            {
                return BadRequest("Room type is required.");
            }

            if (room.RoomType.Length > 50)
            {
                return BadRequest("Room type cannot exceed 50 characters.");
            }

            // XSS защита за RoomType
            room.RoomType = InputValidator.SanitizeInput(room.RoomType);

            // Проверка за валидност на EstateId
            var estate = await _context.Estates.FindAsync(estateId);
            if (estate == null)
            {
                return NotFound("The specified estate does not exist.");
            }

            room.EstateId = estateId;

            try
            {
                // Добавяне на стаята в базата данни чрез SecureRepository
                await _secureRepository.AddEntityAsync(room);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add room for estate {estateId}: {ex.Message}");
                return StatusCode(500, $"An error occurred while adding the room: {ex.Message}");
            }

            // Успешен отговор
            return Ok("Room added successfully.");

            /*
            room.RoomType = InputValidator.SanitizeInput(room.RoomType); // XSS защита
            room.EstateId = estateId;

            await _secureRepository.AddEntityAsync(room);
            return Ok("Room added successfully.");

            //_context.Rooms.Add(room);
            //_context.SaveChanges();
            */
        }

        [Authorize(Roles = "EstateOwner")]
        [HttpPost("edit")]
        public IActionResult Edit([FromBody] Room room)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRoom = _context.Rooms.FirstOrDefault(r => r.Id == room.Id);

            if (existingRoom == null)
            //if (!_context.Rooms.Any(r => r.Id == room.Id))
            {
                return BadRequest("room doesn't exist");
            }

           // Room room1 = _context.Rooms.Where(r => r.Id == room.Id).First();
            existingRoom.RoomType = room.RoomType;
            existingRoom.BedCount = room.BedCount;

            _context.Update(existingRoom);// Променлива room1 - стара
            _context.SaveChanges();

            return Ok();
        }


        [Authorize(Roles = "EstateOwner")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
                return BadRequest("Room doesn't exist");

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return Ok();
            //if (!_context.Rooms.Any(r => r.Id == id))
            //{
            //    return BadRequest("room doesn't exist");
            //}
            //Room room = _context.Rooms.Where(r => r.Id == id).First();
            //_context.Rooms.Remove(room);
            //_context.SaveChanges();
            //return Ok();
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
