using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstateController : Controller
    {
        private readonly PropertyManagementContext _context;

        public EstateController(PropertyManagementContext context)
        {
            _context = context;
        }
        [HttpPost("create")]
        public IActionResult Create([FromBody] Estate estate)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            //if (_context.Estates.Any(u => u.Title == estate.Title))
            //{
            //    return BadRequest();
            //}

            //трябва да се сложи id-то на owner-a, не съм сигурен как

            _context.Add(estate);
            _context.SaveChanges();



            return Ok();
        }
        [HttpPost("edit")]
        public IActionResult Edit([FromBody] Estate estate)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            //if (_context.Estates.Any(u => u.Title == estate.Title))
            //{
            //    return BadRequest();
            //}
            Estate e = _context.Estates.Where(e => e.Id == estate.Id).First();
            e.Title = estate.Title;
            e.PricePerNight = estate.PricePerNight;
            e.Location = estate.Location;
            //etc. etc.
            _context.Update(e);
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if(!_context.Estates.Any(e=>e.Id==id))
            {
                return BadRequest();
            }
            Estate estate = _context.Estates.Where(e=>e.Id==id).First();
            _context.Remove(estate);
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estate>>> GetAllEstates()
        {
            try
            {
                var users = await _context.Estates.ToListAsync();
                return Ok(users); 
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
