using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;
using ManageServer.Data;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

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
        public ActionResult Register()
        {
            string guestLoginId = Guid.NewGuid().ToString();  // UUID를 사용하여 유니크한 Login ID 생성
            string guestPassword = GenerateSecureRandomPassword();  // 간단한 랜덤 패스워드 생성 함수 호출

            // LoginId 중복 확인 (혹시나 발생할 수 있는 충돌을 방지)
            var existingUser = _context.Users.FirstOrDefault(u => u.LoginId == guestLoginId);
            if (existingUser != null)
            {
                do
                {
                    guestLoginId = Guid.NewGuid().ToString();
                    existingUser = _context.Users.FirstOrDefault(u => u.LoginId == guestLoginId);
                }
                while (existingUser != null);
            }

            var user = new User
            {
                LoginId = guestLoginId,
                Password = HashingHelper.HashPassword(guestPassword)  // 비밀번호 해싱
            };

            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
                return Ok(new { 
                    LoginId = guestLoginId, 
                    Password = guestPassword, 
                    Message = "Guest user registered successfully!" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error registering guest user: " + ex.Message);
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

            if (!IsValidPassword(user.Password))
            {
                return BadRequest("Invalid login ID or password.");
            }

            if (!HashingHelper.VerifyPassword(user.Password, loginUser.Password))
            {
                return Unauthorized(new { message = "Invalid password." });
            }

            return Ok(new { message = "Login successful.", UserId = user.Id });
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8 || password.Length > 30)
                return false;
            return true;
        }

        private string GenerateSecureRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes); // 무작위 바이트를 배열에 채움
                return new string(bytes.Select(b => validChars[b % validChars.Length]).ToArray());
            }
        }
    }
}
