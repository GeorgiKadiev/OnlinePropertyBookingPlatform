using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;
using OnlinePropertyBookingPlatform.Models.DataTransferObjects;
using OnlinePropertyBookingPlatform.Utility;


namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstateController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly InputSanitizer _sanitizer;

        public EstateController(PropertyManagementContext context, InputSanitizer sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }

        private static readonly List<string> AvailableAmenities = new List<string>
    {
        "Wi-Fi",
        "Parking",
        "Swimming Pool",
        "Eco-Friendly",
        "Hair Dryer",
        "Fridge",
        "Fireplace",
        "Air Conditioning",
        "Pet-Friendly",
        "Digital Nomad Friendly",
        "Coffe Maker",
        "Balcony",
        "Kitchen",
        "Fitness Centre"
    };

        [HttpGet("available-amenities")]
        public IActionResult GetAvailableAmenities()
        {
            return Ok(AvailableAmenities); // Send hardcoded amenities to the frontend
        }


        [Authorize(Roles = "EstateOwner")]
        [HttpPost("create")]
        public IActionResult Create([FromBody] Estate estate)
        {
            if (_context.Estates.Any(u => u.Title == estate.Title))
            {
                return BadRequest("Estate title already exists");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Санитизация на входните данни
            estate.Title = _sanitizer.Sanitize(estate.Title);
            estate.Location = _sanitizer.Sanitize(estate.Location);
            estate.Description = _sanitizer.Sanitize(estate.Description);

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            // ID на собственика на имота
            estate.EstateOwnerId = int.Parse(userIdClaim.Value);
            _context.Add(estate);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "EstateOwner")]
        [HttpPost("edit")]
        public IActionResult Edit([FromBody] Estate estate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var estateToEdit = _context.Estates.FirstOrDefault(e => e.Id == estate.Id);
            if (estateToEdit == null)
            {
                return BadRequest("Estate doesn't exist.");
            }

            // Санитизация на входните данни
            estateToEdit.Title = _sanitizer.Sanitize(estate.Title);
            estateToEdit.Location = _sanitizer.Sanitize(estate.Location);
            estateToEdit.Description = _sanitizer.Sanitize(estate.Description);
            estateToEdit.PricePerNight = estate.PricePerNight;

            _context.Update(estateToEdit);
            _context.SaveChanges();
            return Ok();

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            //if (_context.Estates.Any(u => u.Title == estate.Title))
            //{
            //    return BadRequest();
            //}
            /*
            Estate e = _context.Estates.Where(e => e.Id == estate.Id).First();
            e.Title = estate.Title;
            e.PricePerNight = estate.PricePerNight;
            e.Location = estate.Location;
            //etc. etc.
            _context.Update(e);
            _context.SaveChanges();
            return Ok();
            */
        }

        [Authorize(Roles = "EstateOwner")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_context.Estates.Any(e => e.Id == id))
            {
                return BadRequest();
            }
            Estate estate = _context.Estates.Where(e => e.Id == id).First();
            _context.Remove(estate);
            _context.SaveChanges();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-estates")]
        public async Task<ActionResult<IEnumerable<EstateDto>>> GetAllEstates()
        {
            try
            {
                var users = await _context.Estates
                    .Include(e => e.EstateOwner) // Include EstateOwner
                    .Include(e => e.Amenities)
                    .Include(e => e.Photos)// Include Amenities
                    .Select(e => new EstateDto
                    {
                        Id = e.Id,
                        Location = e.Location,
                        Title = e.Title,
                        PricePerNight = e.PricePerNight,
                        EstateOwnerId = e.EstateOwnerId,
                        Description = e.Description,
                        EstateOwnerName = e.EstateOwner.Username,
                        Amenities = e.Amenities.Select(a => a.AmenityName).ToList(),
                        Photos = e.Photos.Select(p => p.Url).ToList()
                    }).ToListAsync();


                return Ok(users); // Return the list of DTOs
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "EstateOwner")]
        [HttpGet("{id}/reservations")]
        public async Task<ActionResult<List<ReservationDto>>> GetEstateReservations(int id)
        {
            if (!_context.Estates.Any(e => e.Id == id))
                return BadRequest("Estate doesn't exist");

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (_context.Estates.First(e => e.Id == id).EstateOwnerId != int.Parse(userIdClaim.Value))
                return BadRequest("You don't have access to this information");

            var reservations = await _context.Reservations
                    .Where(r => r.EstateId == id)
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
        [Authorize]
        [HttpGet("{estateId}/rooms")]
        public async Task<ActionResult<List<RoomDto>>> GetRooms(int estateId)
        {
            if (!_context.Estates.Any(e => e.Id == estateId))
            {
                return BadRequest("Estate not found.");
            }

            var rooms = _context.Rooms
                .Where(r => r.EstateId == estateId)
                .Include(r => r.Estate)
                .Include(r => r.Photos)
                .Select(room => new RoomDto
                {
                    Id = room.Id,
                    Name = room.Name,
                    Description = room.Description,
                    MaxGuests = room.MaxGuests,
                    BedCount = room.BedCount,
                    RoomType = room.RoomType,
                    EstateId = room.EstateId,
                    EstateName = room.Estate.Title,
                    Photos = room.Photos.Select(p => p.Url).ToList()
                });
            return Ok(rooms);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EstateDto>> GetEstateDetails(int id)
        {
            try
            {
                var e = await _context.Estates
                    .Include(e => e.Amenities)
                    .Include(e => e.Photos)// Include Amenities
                    .FirstOrDefaultAsync(e => e.Id == id); // Corrected query to fetch related data

                if (e == null)
                {
                    return NotFound($"Estate with ID {id} not found.");
                }

                var dto = new EstateDto
                {
                    Id = e.Id,
                    Location = e.Location,
                    Title = e.Title,
                    PricePerNight = e.PricePerNight,
                    EstateOwnerId = e.EstateOwnerId,
                    Description = e.Description,
                    EstateOwnerName = _context.Users
                        .FirstOrDefault(u => u.Id == e.EstateOwnerId)?.Username, // Ensure EstateOwnerName is set
                    Amenities = e.Amenities.Select(a => a.AmenityName).ToList(),
                    Photos = e.Photos.Select(e => e.Url).ToList()// Map Amenities
                };

                return Ok(dto); // Return the DTO
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize]
        [HttpGet("owner-estates/{id}")]
        public async Task<ActionResult<List<EstateDto>>> GetOwnerEstates(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return BadRequest("Estate owner not found");
            }
            var users = await _context.Estates
                    .Where(e => e.EstateOwnerId == id)
                    .Include(e => e.EstateOwner)
                    .Include(e => e.Photos)
                    .Select(e => new EstateDto
                    {
                        Id = e.Id,
                        Location = e.Location,
                        Title = e.Title,
                        PricePerNight = e.PricePerNight,
                        EstateOwnerId = e.EstateOwnerId,
                        Description = e.Description,
                        EstateOwnerName = e.EstateOwner.Username,
                        Photos = e.Photos.Select(p => p.Url).ToList()

                    }).ToListAsync();
            return Ok(users);


        }

        [Authorize(Roles = "EstateOwner")]
        [HttpPost("{id}/add-photo")]
        public async Task<ActionResult> SetEstatePhoto(int id, [FromBody] UrlModel model)
        {
            if (!_context.Estates.Any(e => e.Id == id))
            {
                return BadRequest("Estate not found");
            }
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            int userid = int.Parse(userIdClaim.Value);
            var estates = _context.Estates.Where(e => e.Id == id);
            var estateownerid = estates.First().EstateOwnerId;
            if (estateownerid != userid)
                return BadRequest("You don't have access to this");
            var Photo = new EstatePhoto
            {
                Url = model.Url,
                EstateId = id,

            };

            _context.EstatesPhotos.Add(Photo);
            _context.SaveChanges();
            return Ok();


        }
        // New Code Starts Here
        [Authorize(Roles = "EstateOwner")]
        [HttpPost("rooms/create")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDto roomDto)
        {
            // Validate the request
            if (string.IsNullOrEmpty(roomDto.Name) || roomDto.MaxGuests <= 0)
            {
                return BadRequest("Invalid room details.");
            }

            // Create a new room
            var newRoom = new Room
            {
                Name = roomDto.Name,
                Description = roomDto.Description,
                MaxGuests = roomDto.MaxGuests,
                BedCount = roomDto.BedCount,
                RoomType = roomDto.RoomType,
                EstateId = roomDto.EstateId, // Link the room to an estate
                Amenities = new List<Amenity>()
            };

            // Add selected amenities to the room
            foreach (var amenityName in roomDto.Amenities)
            {
                var amenity = await _context.Amenities.FirstOrDefaultAsync(a => a.AmenityName == amenityName);

                if (amenity != null)
                {
                    newRoom.Amenities.Add(amenity); // Associate the amenity with the room
                }
            }

            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();

            return Ok("Room created successfully.");
        }


        [Authorize]
        [HttpGet("get-amenities")]
        public async Task<ActionResult<List<string>>> GetAllAmenities()
        {
            var amenities = await _context.Amenities
                .Select(a => a.AmenityName)
                .ToListAsync();

            return Ok(amenities); // this will return a list of amenities
        }




        [HttpPost("filter")]
        public async Task<ActionResult<List<EstateDto>>> GetFilteredEstates([FromBody] EstateFilterDto filter)
        {
            try
            {
                var estates = _context.Estates
                    .Include(e => e.Amenities)
                    .Include(e => e.Reservations) // Include reservations for date filtering
                    .AsQueryable();

                if (!string.IsNullOrEmpty(filter.Location))
                {
                    estates = estates.Where(e => e.Location.Contains(filter.Location));
                }

                if (filter.MinPrice.HasValue)
                {
                    estates = estates.Where(e => (decimal)e.PricePerNight >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    estates = estates.Where(e => (decimal)e.PricePerNight <= filter.MaxPrice.Value);
                }

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    var startDate = DateOnly.FromDateTime(filter.StartDate.Value);
                    var endDate = DateOnly.FromDateTime(filter.EndDate.Value);

                    estates = estates.Where(e => !e.Reservations.Any(r =>
                        r.CheckInDate <= endDate && r.CheckOutDate >= startDate));
                }

                if (filter.Amenities != null && filter.Amenities.Any())
                {
                    foreach (var amenity in filter.Amenities)
                    {
                        estates = estates.Where(e => e.Amenities.Any(a => a.AmenityName == amenity));
                    }
                }

                var result = await estates
                    .Include(e => e.Amenities)
                    .Include(e => e.Photos)
                    .Select(e => new EstateDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Location = e.Location,
                        PricePerNight = e.PricePerNight,
                        Amenities = e.Amenities.Select(a => a.AmenityName).ToList(),
                        Photos = e.Photos.Select(p => p.Url).ToList()
                    }).ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

