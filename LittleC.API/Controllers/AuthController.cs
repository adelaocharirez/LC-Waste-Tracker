// Handles user authentication — getting staff list and PIN login
// Routes:
//   GET  /api/auth/users- returns all active staff members
//   POST /api/auth/login-validates user ID + PIN, returns user info

using LittleC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Returns all active staff members (name + role only, no PIN)
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .Select(u => new { u.Id, u.Name, u.Role })
                .ToListAsync();
            return Ok(users);
        }

        // Accepts userId + PIN, returns user info if valid
        // Returns 401 Unauthorized if PIN is wrong
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId
                    && u.PIN == request.PIN
                    && u.IsActive);

            if (user == null)
                return Unauthorized(new { message = "Invalid PIN" });

            return Ok(new { user.Id, user.Name, user.Role, message = "Login successful" });
        }
    }

    // Request model for login endpoint
    public class LoginRequest
    {
        public int UserId { get; set; }
        public string PIN { get; set; } = string.Empty;
    }
}