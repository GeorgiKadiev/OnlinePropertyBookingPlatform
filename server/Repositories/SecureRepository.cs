using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;


namespace OnlinePropertyBookingPlatform.Repositories
{
    public class SecureRepository
    {
        private readonly PropertyManagementContext _context;

        public SecureRepository(PropertyManagementContext context)
        {
            _context = context;
        }

        // Fetching users by role with parameterized query
        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => EF.Functions.Like(u.Role, role))
                .ToListAsync();
        }

        // Fetch reservations by user ID securely
        public async Task<List<Reservation>> GetReservationsByUserIdAsync(Guid userId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        // Add a new reservation securely
        public async Task<bool> AddReservationAsync(Reservation reservation)
        {
            try
            {
                await _context.Reservations.AddAsync(reservation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log error for debugging (not exposing sensitive details to end users)
                Console.WriteLine($"Error adding reservation: {ex.Message}");
                return false;
            }
        }

        // Update any entity securely
        public async Task<bool> UpdateEntityAsync<T>(T entity) where T : class
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log error for debugging
                Console.WriteLine($"Error updating entity: {ex.Message}");
                return false;
            }
        }

        // Delete an entity securely
        public async Task<bool> DeleteEntityAsync<T>(Guid id) where T : class
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity == null) return false;

                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting entity: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Reservation>> GetReservationsForRoom(Guid roomId)
        {
            return await _context.Reservations
                .Where(r => r.RoomId == roomId)
                .ToListAsync();
        }
        // Integrating with UserController - Fetch users by role
        public async Task<List<User>> FetchUsersByRole(string role)
        {
            return await GetUsersByRoleAsync(role);
        }

        // Integrating with ReservationController - Add new reservation
        public async Task<bool> AddNewReservation(Reservation reservation)
        {
            return await AddReservationAsync(reservation);
        }

        // Secure retrieval of property details
        public async Task<Estate> GetPropertyDetails(int estateId)
        {
            return await _context.Estates
                .Include(e => e.Amenities)
                .FirstOrDefaultAsync(e => e.Id == estateId);
        }

        public async Task<bool> AddEntityAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity); // Добавяне на обекта към базата данни
            await _context.SaveChangesAsync(); // Запазване на промените
            return true; // Връщане на успех
        }
        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }
    }
}
