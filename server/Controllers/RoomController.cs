﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;
using OnlinePropertyBookingPlatform.Models.DataTransferObjects;
using OnlinePropertyBookingPlatform.Utility;
using System.Security.Cryptography.X509Certificates;

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
        [Authorize(Roles = "EstateOwner")]
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
            room.RoomType = _sanitizer.Sanitize(room.RoomType);
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return Ok();

        }
        [Authorize(Roles = "EstateOwner")]
        [HttpPost("edit")]
        public IActionResult Edit([FromBody] Room room)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_context.Rooms.Any(r => r.Id == room.Id))
            {
                return BadRequest("room doesn't exist");
            }
            var roomToEdit = _context.Rooms.FirstOrDefault(r => r.Id == room.Id);

            // Санитизация на входните данни
            roomToEdit.Name = _sanitizer.Sanitize(room.Name); // Санитизация на името на стаята
            roomToEdit.Description = _sanitizer.Sanitize(room.Description);
            if(room.RoomType != null)
            roomToEdit.RoomType = _sanitizer.Sanitize(room.RoomType);
            if(room.MaxGuests!=null)// Санитизация на името на стаята
            roomToEdit.MaxGuests = int.Parse(_sanitizer.Sanitize(room.MaxGuests.ToString()));
            if(room.BedCount!=null) 
            roomToEdit.BedCount =  int.Parse(_sanitizer.Sanitize(room.BedCount.ToString())); // Санитизация на името на стаята// Санитизация на описанието

            Room room1 = _context.Rooms.Where(r => r.Id == room.Id).First();

            _context.Update(roomToEdit);
            _context.SaveChanges();

            return Ok();
        }
        [Authorize(Roles = "EstateOwner,Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet("{estateId}")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRoomsForAnEstate(int estateId)
        {
            try
            {
                if(!_context.Users.Any(e=>e.Id == estateId))
                {
                    return BadRequest("No estate with given id");
                }
                var rooms = await _context.Rooms.Where(r=>r.EstateId==estateId).ToListAsync();
                if (rooms.Count == 0)
                    return BadRequest("Estate doesn't have any rooms");
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("details/{roomId}")]
        public async Task<ActionResult<Room>> GetRoomDetails(int roomId)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(roomId);
                if (room == null)
                {
                    return NotFound($"room with ID {roomId} not found.");
                }

                RoomDto dto = new RoomDto()
                {
                    Id = room.Id,
                    Name = room.Name,
                    Description = room.Description,
                    MaxGuests = room.MaxGuests,
                    BedCount = room.BedCount,
                    RoomType = room.RoomType,
                    EstateId = room.EstateId
                };
                string EstateName = _context.Estates.FirstOrDefault(e => e.Id == dto.EstateId).Title;
                if(EstateName ==null)
                {
                    return NotFound("The room doesn't match a proper estate");
                }
                List<DateOnly> occupied = _context.Reservations
                .Where(r => r.RoomId == roomId)
                .SelectMany(r => Enumerable.Range(0, (r.CheckOutDate.ToDateTime(TimeOnly.MinValue) - r.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days)
                .Select(offset => r.CheckInDate.AddDays(offset)))
                .ToList();


                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("details/{roomId}/dates")]
        public async Task<ActionResult<List<DateOnly>>> GetEmptyDates(int roomId)
        {
            List<DateOnly> occupied = _context.Reservations
                .Where(r => r.RoomId == roomId)
                .SelectMany(r => Enumerable.Range(0, (r.CheckOutDate.ToDateTime(TimeOnly.MinValue) - r.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days)
                .Select(offset => r.CheckInDate.AddDays(offset)))
                .ToList();
            return Ok();
        }
        [Authorize(Roles = "EstateOwner")]
        [HttpPost("{estateId}/{ roomId}/add-photo")]
        public async Task<ActionResult> SetEstatePhoto(int roomId,int estateId, [FromBody] UrlModel model)
        {
            if (!_context.Rooms.Any(e => e.Id == roomId))
            {
                return BadRequest("Room not found");
            }
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            int userid = int.Parse(userIdClaim.Value);
            var rooms = _context.Rooms.Where(e => e.Id == roomId);
            if(!_context.Estates.Any(e=>e.Id ==estateId))
            {
                return BadRequest("Estate doesn't exist");
            }
            var estateownerid = _context.Estates.First(e => e.Id == estateId).EstateOwnerId;
            if (estateownerid != userid)
                return BadRequest("You don't have access to this");
            var Photo = new RoomPhoto
            {
                Url = model.Url,
                Id = roomId,
            };

            _context.RoomPhotos.Add(Photo);
            _context.SaveChanges();
            return Ok();


        }


    }
}
