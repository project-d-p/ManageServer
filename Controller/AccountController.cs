using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;
using ManageServer.Data;
using System.Text.RegularExpressions;

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

            if (!IsValidLoginId(user.LoginId) || !IsValidPassword(user.Password))
            {
                return BadRequest("Invalid login ID or password.");
            }

            // LoginId 중복 확인
            var existingUser = _context.Users.FirstOrDefault(u => u.LoginId == user.LoginId);
            if (existingUser != null)
            {
                return Conflict("Login ID already exists.");
            }

            user.Password = HashingHelper.HashPassword(user.Password);
            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
                return StatusCode(201, new { message = "User registered successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest("Error registering user: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] User loginUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.LoginId == loginUser.LoginId);

            // User 데이터가 null이거나 Login ID 또는 Password가 비어있는지 확인
            if (user == null || string.IsNullOrEmpty(user.LoginId) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(loginUser.Password))
            {
                return NotFound(new { message = "User not found." });
            }

            if (!IsValidLoginId(user.LoginId) || !IsValidPassword(user.Password))
            {
                return BadRequest("Invalid login ID or password.");
            }

            if (!HashingHelper.VerifyPassword(user.Password, loginUser.Password))
            {
                return Unauthorized(new { message = "Invalid password." });
            }

            return Ok(new { message = "Login successful.", UserId = user.Id });
        }

        private bool IsValidLoginId(string loginId)
        {
            return !string.IsNullOrEmpty(loginId) && loginId.Length >= 8 && loginId.Length <= 30;
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8 || password.Length > 30)
                return false;

            // 비밀번호 복잡성 확인: 대문자, 소문자, 숫자, 특수 문자 포함
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,30}$";
            return Regex.IsMatch(password, passwordPattern);
        }
    }
}
