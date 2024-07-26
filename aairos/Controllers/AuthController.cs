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
/*        private readonly FileLoggerService _logger;
*/        private readonly LoginContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(LoginContext context, IConfiguration configuration, FileLoggerService logger)
        {
/*            _logger = logger;
*/            _context = context;
            _configuration = configuration;
        }

        // New method to get UserProfileId by LoginId
        [HttpGet("login/{loginId}")]
        public async Task<IActionResult> GetUserProfileIdByLoginId(int loginId)
        {
            var user = await _context.Login.FindAsync(loginId);

            if (user == null)
            {
/*                await _logger.LogAsync($"GET: api/Auth/login/{loginId} - User not found.");
*/                return NotFound("User not found");
            }

/*            await _logger.LogAsync($"GET: api/Auth/login/{loginId} - UserProfileId: {user.UserProfileId}.");
*/            return Ok(new { UserProfileId = user.UserProfileId });
        }

        // This is a login fetch method by entering username and password
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
/*                await _logger.LogAsync("POST: api/Auth/login - Invalid request.");
*/                return BadRequest("Invalid request");
            }

            var user = await _context.Login.FirstOrDefaultAsync(u => u.UserName == login.Username && u.Password == login.Password);

            if (user == null)
            {
/*                await _logger.LogAsync($"POST: api/Auth/login - Unauthorized attempt for username: {login.Username}.");
*/                return Unauthorized();
            }

/*            await _logger.LogAsync($"POST: api/Auth/login - Successful login for username: {login.Username}, loginId: {user.LoginId}, IsAdmin: {user.IsAdmin}.");
*/            return Ok(new
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
/*                await _logger.LogAsync("POST: api/Auth/register - Invalid request.");
*/                return BadRequest("Invalid request");
            }

            var existingUser = await _context.Login.FirstOrDefaultAsync(u => u.UserName == newUser.Username);
            if (existingUser != null)
            {
/*                await _logger.LogAsync($"POST: api/Auth/register - Username already exists: {newUser.Username}.");
*/                return BadRequest("Username already exists");
            }

            var user = new Login
            {
                UserName = newUser.Username,
                Password = newUser.Password,
                IsAdmin = newUser.IsAdmin 
            };

            _context.Login.Add(user);
            await _context.SaveChangesAsync();

/*            await _logger.LogAsync($"POST: api/Auth/register - User registered successfully, loginId: {user.LoginId}.");
*/            return Ok(new { message = "User registered successfully", loginId = user.LoginId });
        }

        // This is a PUT method for login
        [HttpPut("update/{LoginId}")]
        public async Task<IActionResult> UpdateUser(int LoginId, [FromBody] UserLogin updatedUser)
        {
            if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Username) || string.IsNullOrEmpty(updatedUser.Password))
            {
/*                await _logger.LogAsync("PUT: api/Auth/update - Invalid request.");
*/                return BadRequest("Invalid request");
            }

            var user = await _context.Login.FindAsync(LoginId);

            if (user == null)
            {
/*                await _logger.LogAsync($"PUT: api/Auth/update/{LoginId} - User not found.");
*/                return NotFound("User not found");
            }

            user.UserName = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.IsAdmin = updatedUser.IsAdmin;

            _context.Login.Update(user);
            await _context.SaveChangesAsync();

/*            await _logger.LogAsync($"PUT: api/Auth/update/{LoginId} - User updated successfully.");
*/            return Ok(new { message = "User updated successfully", loginId = user.LoginId });
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return password == hashedPassword;
        }

        /* private string GenerateJwtToken(int loginId, string username)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim("LoginId", loginId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/
    }

    public class UserLogin
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
