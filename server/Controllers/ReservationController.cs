﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OnlinePropertyBookingPlatform.Repositories;


namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : Controller
    {
        private readonly PropertyManagementContext _context;
        private readonly CrudRepository<Reservation> _reservationRepository;


        public ReservationController(PropertyManagementContext context, CrudRepository<Reservation> reservationRepository)
        {
            _context = context;
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        }

        // Създаване на резервация (само клиенти)
        [Authorize(Roles = "Customer")]
        [HttpPost("{estateId}")]
        public IActionResult Create([FromBody]Reservation reservation, int estateId)
        {
            //тук трябва да се добави и Id-то на потребителят,
            //който създава резервацията
            var userId = User.FindFirst("UserId")?.Value; // ID на текущия потребител
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            reservation.CustomerId = int.Parse(userId);//
            reservation.EstateId = estateId;

            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            return Ok();
            

        }
        // Редактиране на резервация (само собственици и администратори)
        [Authorize(Roles = "EstateOwner,Admin")]
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] Reservation reservation)
        {
            // нов вариант
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reservationToEdit = await _reservationRepository.GetByIdAsync(reservation.Id);
            if (reservationToEdit == null)
            {
                return BadRequest("Reservation doesn't exist.");
            }

            reservationToEdit.CheckInDate = reservation.CheckInDate;
            reservationToEdit.CheckOutDate = reservation.CheckOutDate;

            await _reservationRepository.UpdateAsync(reservationToEdit); // Използвайте CRUD репозитория за актуализация
            return Ok();

            
        }
        // Изтриване на резервация (само администратори)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //добавено Панчо
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return BadRequest("Reservation doesn't exist.");
            }

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
            return Ok();


            
        }

        // Извличане на всички резервации (само администратори)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllReservations()
        {

            try
            {
                var reservations = await _context.Reservations.ToListAsync();
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
        public async Task<ActionResult<IEnumerable<User>>> GetAllReservationsFromAnUser(int userId)
        {
            try
            {
                var reservations = await _context.Reservations.Where(r=>r.CustomerId==userId).ToListAsync();
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
        public async Task<ActionResult<Estate>> GetReservationDetails(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);

                if (reservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        //Създаване на обяви()
        [Authorize(Roles = "EstateOwner,Admin")]
        [HttpPost("create")]
        public IActionResult CreateEstate([FromBody] Estate estate)
        {
            var userId = User.FindFirst("UserId")?.Value;
            estate.EstateOwnerId = int.Parse(userId);

            _context.Estates.Add(estate);
            _context.SaveChanges();
            return Ok();
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reservationRepository.AddAsync(reservation);
            return CreatedAtAction(nameof(CreateReservation), new { id = reservation.Id }, reservation);
        }



    }
}
