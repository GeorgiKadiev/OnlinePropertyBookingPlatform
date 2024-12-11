using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly Utility.IEmailSender _emailSender;
        private readonly PropertyManagementContext _context;
        private readonly IConfiguration _config; //добавям за generateJwtToken


        public UserController(PropertyManagementContext context, Utility.IEmailSender emailSender,IConfiguration config)
        {
            _config = config;
            _emailSender = emailSender;
            _context = context;

        }

        private string GenerateJwtToken(User user)
        {
            // Проверка дали Jwt:Key е зададен
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT ключът не е конфигуриран.");
            }

            // Създаване на SymmetricSecurityKey
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Дефиниране на claims (атрибути в токена)
            var claims = new[]
            {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("UserId", user.Id.ToString())
    };

            // Създаване на токена
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            // Връщане на токена като низ
            return new JwtSecurityTokenHandler().WriteToken(token);
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
        //Редактиран вариант ПАНЧО
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || user.Password != model.Password)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        /* ТОВА Е СТАРИЯТ ВАРИАНТ
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
        */
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Generate a reset token (can be a GUID or JWT)
            var resetToken = Guid.NewGuid().ToString();

            // Save token to database (or send it via email directly)
            user.ResetPasswordToken = resetToken;
            _context.SaveChanges();

            // Send email
            var resetLink = $"https://yourapp.com/reset-password?token={resetToken}";
            await _emailSender.SendEmailAsync(email, "Reset Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return Ok("Password reset link sent to your email.");
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(string token, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetPasswordToken == token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }

            user.Password = newPassword;
            user.ResetPasswordToken = null; // Clear the token
            _context.SaveChanges();

            return Ok("Password reset successfully.");
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
