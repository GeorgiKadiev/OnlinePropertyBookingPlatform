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
        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] User user)
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
            //за редактиране
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", "hello");


            return Ok();
        }
        [HttpPost("edit")]
        public IActionResult Edit([FromBody] User user)
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
        public IActionResult Login([FromBody]LoginModel model)
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
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody]RegisterModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return BadRequest("Email is already in use");
            }
            if(model.password1!=model.password2)
            {
                return BadRequest("Passwords don't match");
            }
            User user = new User() 
            { 
                Email = model.Email,
                Password = model.password1,
                Role = "Customer"
            };
            _context.Add(user);
            _context.SaveChanges();
            // да се направи автентикация тук
            await _emailSender.SendEmailAsync(model.Email, "Confirm your email", "hello");
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
