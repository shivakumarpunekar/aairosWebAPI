using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Services;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoginContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(LoginContext context, IConfiguration configuration, FileLoggerService logger)
        {
            _context = context;
            _configuration = configuration;
        }

        // New method to get userProfileId by LoginId
        [HttpGet("login/{loginId}")]
        public async Task<IActionResult> GetuserProfileIdByLoginId(int loginId)
        {
            var user = await _context.Login.FindAsync(loginId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new { userProfileId = user.userProfileId });
        }

        // This is a login fetch method by entering username and password
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Invalid request");
            }

            var user = await _context.Login.FirstOrDefaultAsync(u => u.UserName == login.Username && u.Password == login.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                loginId = user.LoginId,
                IsAdmin = user.IsAdmin,
                username = user.UserName
            });
        }

        // This is a login registration method
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserLogin newUser)
        {
            if (newUser == null || string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.Password))
            {
                return BadRequest("Invalid request");
            }

            // Check if username already exists in the Login table
            var existingUser = await _context.Login.FirstOrDefaultAsync(u => u.UserName == newUser.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            // Create a new Login entry
            var login = new Login
            {
                UserName = newUser.Username,
                Password = newUser.Password,
                IsAdmin = newUser.IsAdmin
            };

            _context.Login.Add(login);
            await _context.SaveChangesAsync();

            // Create a corresponding userprofile entry
            var userProfile = new userprofile
            {
                userProfileId = login.LoginId, // Set UserProfileId same as LoginId
                UserName = newUser.Username,
                Password = newUser.Password,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _context.userprofile.Add(userProfile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully", loginId = login.LoginId });
        }

        // This is a PUT method for login
        [HttpPut("update/{LoginId}")]
        public async Task<IActionResult> UpdateUser(int LoginId, [FromBody] UserLogin updatedUser)
        {
            if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Username) || string.IsNullOrEmpty(updatedUser.Password))
            {
                return BadRequest("Invalid request");
            }

            var user = await _context.Login.FindAsync(LoginId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.UserName = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.IsAdmin = updatedUser.IsAdmin;

            _context.Login.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully", loginId = user.LoginId });
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return password == hashedPassword;
        }
    }

    public class UserLogin
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}