using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;
using ManageServer.Data;
using MatchingClient.Services;

namespace MatchingClient.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public ActionResult Register(User user)
        {
            if (string.IsNullOrEmpty(user.LoginId) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid login ID or password.");
            }

            user.Password = HashingHelper.HashPassword(user.Password);
            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
                Console.WriteLine($"User registered successfully: {user.LoginId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return BadRequest("Error registering user");
            }

            var users = _context.Users.ToList();
            return Ok(new { message = "User registered successfully!", AllUsers = users });
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] User loginUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.LoginId == loginUser.LoginId);
            if (user == null || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(loginUser.Password))
            {
                return NotFound(new { message = "User not found." });
            }

            if (!HashingHelper.VerifyPassword(user.Password, loginUser.Password))
            {
                return BadRequest(new { message = "Invalid password." });
            }

            return Ok(new { message = "Login successful.", UserId = user.Id });
        }
    }
}
