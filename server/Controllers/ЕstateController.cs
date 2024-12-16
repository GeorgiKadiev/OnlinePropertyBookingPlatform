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
    public class EstateController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly SecureRepository _secureRepository;

        public EstateController(PropertyManagementContext context, SecureRepository secureRepository)
        {
            _context = context;
            _secureRepository = secureRepository;
        }



        [Authorize(Roles = "EstateOwner")]
        [ValidateAntiForgeryToken] // CSRF защита
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Estate estate)
        {
            // Проверка за валидност на входните данни
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input data.");
            }

            // Проверка дали Title и Location са валидни (не са празни)
            if (string.IsNullOrEmpty(estate.Title) || string.IsNullOrEmpty(estate.Location))
            {
                return BadRequest("Title and Location are required.");
            }

            // XSS защита
            estate.Title = InputValidator.SanitizeInput(estate.Title);
            estate.Location = InputValidator.SanitizeInput(estate.Location);

            // Проверка за валиден потребител
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            // Присвояване на EstateOwnerId
            estate.EstateOwnerId = int.Parse(userId);

            try
            {
                // Добавяне в базата данни
                await _secureRepository.AddEntityAsync(estate);
            }
            catch (Exception ex)
            {
                // Обработка на грешки
                return StatusCode(500, $"An error occurred while creating the estate: {ex.Message}");
            }

            return Ok("Estate created successfully.");
        }


        [Authorize(Roles = "EstateOwner")]
        [HttpPost("edit")]
        public IActionResult Edit([FromBody] Estate estate)
        {
            var existingEstate = _context.Estates.FirstOrDefault(e => e.Id == estate.Id);
            if (existingEstate == null)
                return BadRequest("Estate doesn't exist");

            existingEstate.Title = estate.Title;
            existingEstate.PricePerNight = estate.PricePerNight;

            _context.Update(existingEstate);
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
            //Estate e = _context.Estates.Where(e => e.Id == estate.Id).First();
            //e.Title = estate.Title;
            //e.PricePerNight = estate.PricePerNight;
            //e.Location = estate.Location;
            //_context.Update(e);
            //_context.SaveChanges();
            //return Ok();
        }
        [Authorize(Roles = "EstateOwner")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var estate = _context.Estates.FirstOrDefault(e => e.Id == id);
            if (estate == null)
                return BadRequest("Estate doesn't exist");

            _context.Estates.Remove(estate);
            _context.SaveChanges();
            return Ok();
            //if(!_context.Estates.Any(e=>e.Id==id))
            //{
            //    return BadRequest();
            //}
            //Estate estate = _context.Estates.Where(e=>e.Id==id).First();
            //_context.Remove(estate);
            //_context.SaveChanges();
            //return Ok();
        }
        [Authorize(Roles = "EstateOwner")]
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Estate>>> GetAllEstates()
        {
            try
            {
                var estates = await _secureRepository.GetAllAsync<Estate>();
                return Ok(estates); // връщаме променливата estates
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Estate>> GetEstateDetails(int id)
        {
            try
            {
                var estate = await _context.Estates.FindAsync(id);

                if (estate == null)
                {
                    return NotFound($"Estate with ID {id} not found.");
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
