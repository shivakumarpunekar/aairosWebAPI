using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using aairos.Data;
using aairos.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using aairos.Handular;
using Microsoft.Extensions.Configuration;


namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly LoginContext _context;
        private readonly IConfiguration _configuration;


        public AuthController(LoginContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            // Check if login data is provided
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Invalid request");
            }

            // Retrieve user from database based on username

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


        //This is a Login registation

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserLogin newUser)
        {
            // Check if user data is provided
            if (newUser == null || string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.Password))
            {
                return BadRequest("Invalid request");
            }

            // Check if username already exists
            var existingUser = await _context.Login.FirstOrDefaultAsync(u => u.UserName == newUser.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            // Create a new user
            var user = new aairos.Model.Login
            {
                UserName = newUser.Username,
                Password = newUser.Password,
                IsAdmin = newUser.IsAdmin
            };

            _context.Login.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully", loginId = user.LoginId });
        }




        //This is a PUT Method for login

        [HttpPut("update/{LoginId}")]
        public async Task<IActionResult> UpdateUser(int LoginId, [FromBody] UserLogin updatedUser)
        {
            // Check if user data is provided
            if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Username) || string.IsNullOrEmpty(updatedUser.Password))
            {
                return BadRequest("Invalid request");
            }

            // Retrieve the user to update
            var user = await _context.Login.FindAsync(LoginId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update user details
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

        /*private string GenerateJwtToken(int loginId, string username)
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
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
