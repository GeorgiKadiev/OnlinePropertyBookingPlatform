using Microsoft.AspNetCore.Authorization;
using OnlinePropertyBookingPlatform.Utility; // Добави namespace за InputSanitizer
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using OnlinePropertyBookingPlatform.Models;
using OnlinePropertyBookingPlatform.Models.DataModels;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OnlinePropertyBookingPlatform.Models.DataTransferObjects;

namespace OnlinePropertyBookingPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly Utility.IEmailSender _emailSender;
        private readonly PropertyManagementContext _context;
        private readonly IConfiguration _config; //добавям за generateJwtToken
        private readonly InputSanitizer _sanitizer; // Добавяме InputSanitizer



        public UserController(PropertyManagementContext context, Utility.IEmailSender emailSender, IConfiguration config, InputSanitizer sanitizer)
        {
            _config = config;
            _emailSender = emailSender;
            _context = context;
            _sanitizer = sanitizer;
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
            // Санитизираме входните данни
            user.Email = _sanitizer.Sanitize(user.Email);
            user.Username = _sanitizer.Sanitize(user.Username);

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
            // Санитизираме входните данни
            user.Email = _sanitizer.Sanitize(user.Email);

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
            User user = _context.Users.FirstOrDefault(u=>u.Id == id);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction();
        }
        [HttpPost("login")]
        //Редактиран вариант ПАНЧО
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Санитизираме входните данни
            model.Email = _sanitizer.Sanitize(model.Email);
            model.Password = _sanitizer.Sanitize(model.Password);

            // Намери потребител по имейл
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized("Invalid email or password");
            }
            if (!user.IsEmailVerified)
                return Unauthorized("Please open your email to verify your account");
            

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

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt"); // Изтрива JWT cookie
            return Ok(new { message = "Successfully logged out" });
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
        public async Task<ActionResult> Register([FromBody] RegisterModel model)
        {

            // Санитизираме входните данни
            model.Email = _sanitizer.Sanitize(model.Email);
            model.password1 = _sanitizer.Sanitize(model.password1);
            model.password2 = _sanitizer.Sanitize(model.password2);

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
                Username = model.Username,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Password = hashedPassword,
                Role = model.Role
            };


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
            var verifyLink = $"http://localhost:5076/api/user/verify-user/{tokenString}";
            user.EmailVerificationToken = tokenString;
            _context.Add(user);
            await _context.SaveChangesAsync(); // Съхраняваме потребителя
            // Изпращане на имейл за потвърждение
            await _emailSender.SendEmailAsync(model.Email, "Confirm your email", $"Click <a href='{verifyLink}'>here</a> to verify your account.");

            // Връщане на токена
            return Ok(new { Token = tokenString, Message = "Registration successful. Please check your email to confirm your account." });

        }
        [HttpGet("verify-user/{token}")]
        public IActionResult RedirectToVerifyUser(string token)
        {
            var client = new HttpClient();
            var response = client.PostAsync($"http://localhost:5076/api/user/verify-user/{token}", null);
            return Ok();
        }

        [HttpPost("verify-user/{token}")]
        public async Task<ActionResult> verifyUser(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.EmailVerificationToken == token);
            if(user == null)
            {
                return BadRequest("Token doesn't exist");
            }
            if(user.IsEmailVerified)
            {
                return BadRequest("User already verified");
            }
            if(IsJwtTokenExpired(token))
            {
                RefreshToken(token);
                user.EmailVerificationToken = token;
                _context.Update(user);
                await _context.SaveChangesAsync(); // Съхраняваме потребителя                                    // Изпращане на имейл за потвърждение
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Click <a href='{token}'>here</a> to verify your account.");
                return Ok("Token expired, new message is sent to your email");
            }
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailModel model)
        {
            // Санитизираме входните данни
            model.Email = _sanitizer.Sanitize(model.Email);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
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
            var resetLink = $"https://yourapp.com/reset-password/{resetToken}";
            await _emailSender.SendEmailAsync("hutchyy@abv.bg", "Reset Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return Ok("Password reset link sent to your email. " + resetToken);
        }

        [HttpPost("reset-password/{token}")]
        public IActionResult ResetPassword(string token,PasswordModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetPasswordToken == token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }


            user.Password = BCrypt.Net.BCrypt.HashPassword(model.newPassword);
            user.ResetPasswordToken = null;
            _context.Update(user);
            _context.SaveChanges();

            return Ok("Password reset successfully."+ token);

        }

        [HttpGet("get-all-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Include(u=>u.Reservations)
                    .Include(u => u.Reviews)
                    .Select(user=> new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        Password = user.Password,
                        Role = user.Role,
                        PhoneNumber = user.PhoneNumber,
                        ReviewCount = user.Reservations.Count(),
                    })
                    .ToListAsync();
                return Ok(users); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserDetails(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                UserDto dto = new UserDto()
                {
                    Id = user.Id,
                    Username= user.Username,
                    Email = user.Email,
                    Password = user.Password,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber
                };
                dto.ReviewCount = _context.Reviews.Where(r=>r.AuthorId == user.Id).Count();


                if (user == null)
                {
                    return NotFound($"Estate with ID {id} not found.");
                }

                return Ok(dto);
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

        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody] LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Username or password is required.");

            // Търсене на потребителя в базата данни
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            // Генериране на JWT токен
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            });
        }
        static bool IsJwtTokenExpired(string token)
        {
            
                var jwtHandler = new JwtSecurityTokenHandler();

                if (!jwtHandler.CanReadToken(token))
                {
                    throw new ArgumentException("Invalid JWT token format.");
                }

                var jwtToken = jwtHandler.ReadJwtToken(token);

                // Extract the expiration time (exp claim)
                DateTime? expiry = jwtToken.ValidTo;

                // Check if the token is expired
                return expiry < DateTime.UtcNow;
            
            
        }

    }
}
