using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;
using Org.BouncyCastle.Crypto.Generators;
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
               issuer: _config["Jwt:Issuer"],
               audience: _config["Jwt:Audience"],
               claims: claims,
               expires: DateTime.Now.AddHours(3),
               signingCredentials: credentials
    );

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

            // Хеширане на новата парола, ако има промяна
            if (!string.IsNullOrEmpty(user.Password))
            {
                user1.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            user1.Role = user.Role;
            _context.Update(user1);
            _context.SaveChanges();

            return Ok();

            /* стар вариант
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
            */
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
            // Намери потребител по имейл
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized("Invalid email or password");
            }

            // Генериране на JWT токен
            var token = GenerateJwtToken(user);
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true, // Prevents access via JavaScript
                Secure = true,   // Ensures the cookie is sent only over HTTPS
                SameSite = SameSiteMode.Strict, // Prevents CSRF by ensuring the cookie is only sent on same-site requests
                Expires = DateTime.UtcNow.AddHours(3) // Sets an expiration time for the cookie
            });
            return Ok(new { Token = token });
        }

        
        // Не съм много сигурен дали е така някой да провери от тук до
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] string token)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            if (principal == null)
            {
                return BadRequest("Invalid token");
            }

            var userId = principal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found.");
            }

            var newToken = GenerateJwtToken(new User
            {
                Id = int.Parse(userId!),
                Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
                Role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!
            });

            return Ok(new { Token = newToken });
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
                    ValidateLifetime = false // Позволява изтекли токени
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
            }
            catch
            {
                // В случай на грешка се връща null
            }

            return null;

            /*  един вариант
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, // You might want to validate the audience and issuer
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false // Allow expired tokens
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
            */
        }

        //до тук :)

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody]RegisterModel model)
        {
            // не съм сигурен за това
                // Проверка дали имейлът вече се използва
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    return BadRequest("Email is already in use");
                }

                // Проверка дали паролите съвпадат
                if (model.password1 != model.password2)
                {
                    return BadRequest("Passwords don't match");
                }

                // Хеширане на паролата с BCrypt
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.password1);

                // Създаване на нов потребител
                User user = new User()
                {
                    Email = model.Email,
                    Password = hashedPassword,
                    Role = model.Role
                };

                _context.Add(user);
                await _context.SaveChangesAsync(); // Съхраняваме потребителя

                // Генериране на JWT токен
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes("your-very-strong-secret-key-of-at-least-32-bytes"); // Използвайте вашия секретен ключ
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // Изпращане на имейл за потвърждение
                await _emailSender.SendEmailAsync(model.Email, "Confirm your email", "hello");

                // Връщане на токена
                return Ok(new { Token = tokenString, Message = "Registration successful. Please check your email to confirm your account." });


            
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
            _context.Update(user);
            _context.SaveChanges();

            // Send email
            var resetLink = $"https://yourapp.com/reset-password?token={resetToken}";
            await _emailSender.SendEmailAsync(email, "Reset Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return Ok("Password reset link sent to your email.");
        }

        [HttpPost("reset-password/{token}")]
        public IActionResult ResetPassword(string token, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetPasswordToken == token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }


            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetPasswordToken = null;
            _context.Update(user);
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
        [Authorize]
        [HttpGet("get-user-id")]
        public IActionResult GetUserId()
        {
            // Extract the UserId claim
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = userIdClaim.Value;

            return Ok(new { UserId = userId });
        }

    }
}
