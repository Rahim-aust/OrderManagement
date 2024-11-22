using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderManagement.Models;

namespace OrderManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ProductManageDbContext context;
        private readonly IConfiguration config;

        public AuthController(ProductManageDbContext context, IConfiguration config)
        {
            this.context = context;
            this.config = config; 
        }

        // API to register
        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            if (await context.TblUsers.AnyAsync(u => u.StrUsername == username))
            {
                return BadRequest("User already exists.");
            }

            // Create a new user
            var user = new TblUser
            {
                StrUsername = username,
                StrPasswordHash = HashPassword(password), 
                IntFailedLoginAttempts = 0, 
                IsLocked = false 
            };

            context.TblUsers.Add(user);
            await context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // API For LogIn
        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (context == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not available.");
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await context.TblUsers.FirstOrDefaultAsync(u => u.StrUsername == username);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            if (user.IsLocked)
            {
                return Unauthorized("Account is locked due to too many failed login attempts.");
            }

            if (!VerifyPassword(password, user.StrPasswordHash))
            {
                user.IntFailedLoginAttempts++;
                if (user.IntFailedLoginAttempts >= 3)
                {
                    user.IsLocked = true;
                }
                await context.SaveChangesAsync();
                return Unauthorized("Invalid username or password.");
            }

            user.IntFailedLoginAttempts = 0;
            await context.SaveChangesAsync();

            var token = GenerateToken(username);

            return Ok(new
            {
                Message = "Login successful. Welcome back!",
                Token = token
            });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private string GenerateToken(string username)
        {
            return $"{username}-token-{Guid.NewGuid()}";
        }
    }
}
