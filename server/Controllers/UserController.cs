using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly Utility.IEmailSender _emailSender;
        private readonly PropertyManagementContext _context;

        public UserController(PropertyManagementContext context, Utility.IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _context = context;
        }
        [HttpPost("index")]
        public async Task<IActionResult> Index()
        {
            var receiver = "del40ismost@gmail.com";
            var subject = "Test";
            var message = "Hello World";
            await _emailSender.SendEmailAsync(receiver,subject,message);
            return Ok();
        }
        [HttpPost("create")]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest("There is an account existing with the given email");
            }
            _context.Add(user);
            _context.SaveChanges();



            return Ok();
        }
        [HttpPost("edit")]
        public IActionResult Edit(User user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Users.Any(u => u.Id == user.Id))
            {
                return BadRequest("User doesn't exist");
            }
            User user1 = _context.Users.Where(u => u.Id == user.Id).First();
            user1.Email = user.Email;
            user1.Password = user.Password;
            user1.Role = user.Role;
            _context.Update(user1);
            _context.SaveChanges();

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_context.Users.Any(u => u.Id == id))
            {
                return BadRequest("User doesn't exist");
            }
            User user = _context.Users.Where(u => u.Id == id).First();
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            if(!_context.Users.Any(u=>u.Email==model.Email))
            {
                return BadRequest("Email is not valid");
            }
            if(_context.Users.Where(u=>u.Email== model.Email).First().Password!=model.Password) 
            {
                return BadRequest("Invalid password");
            }
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Estate>> GetUserDetails(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound($"Estate with ID {id} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
