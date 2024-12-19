﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
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
        [Authorize(Roles = "EstateOwner")]
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Санитизация на входните данни
            estate.Title = _sanitizer.Sanitize(estate.Title);
            estate.Location = _sanitizer.Sanitize(estate.Location);
            estate.Description = _sanitizer.Sanitize(estate.Description);

            // ID на собственика на имота
            estate.EstateOwnerId = int.Parse(User.FindFirst("UserId")?.Value);
            estate.EstateOwnerId = int.Parse(User.FindFirst("UserId")?.Value);

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
            if(!_context.Estates.Any(e=>e.Id==id))
            {
                return BadRequest();
            }
            Estate estate = _context.Estates.Where(e=>e.Id==id).First();
            _context.Remove(estate);
            _context.SaveChanges();
            return Ok();
        }
        [Authorize(Roles = "EstateOwner")]
        [HttpGet("get")]
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
