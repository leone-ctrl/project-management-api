using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectmanagementAPI.Models;
using System.Net;
using System.Net.Mail;

namespace ProjectmanagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ProjectManagementContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(ProjectManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            // Prints the exact database connection string to your Visual Studio Output window
            Console.WriteLine("ACTIVE DB CONNECTION: " + _context.Database.GetConnectionString());
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        // GET: api/Users/details/5
        [HttpGet("details/{id}")]
        public async Task<ActionResult<User>> GetUserDetails(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            return Ok(user);
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginModel loginModel)
        {
            if (string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
            {
                return BadRequest(new { Message = "Email and password are required." });
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == loginModel.Email && u.Password == loginModel.Password);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserDetails), new { id = user.UserId }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest(new { Message = "User ID mismatch." });
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"User with ID {id} successfully deleted." });
        }

        // POST: api/Users/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { Message = "Email is required." });
            }

            string cleanEmail = request.Email.Trim().ToLower();

            // Flexible search that ignores casing and accidental spaces
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == cleanEmail);

            if (user == null)
            {
                return Ok(new { Message = "If an account matches that email, an OTP has been sent." });
            }

            string otpCode = new Random().Next(100000, 999999).ToString();

            // 🔑 PRINT OTP TO CONSOLE FOR INSTANT TESTING
            Console.WriteLine("🔑 ========================================== 🔑");
            Console.WriteLine($"🔑 YOUR RESET OTP CODE FOR {user.Email} IS: {otpCode}");
            Console.WriteLine("🔑 ========================================== 🔑");

            var otpEntry = new PasswordOtp
            {
                Email = user.Email,
                OtpCode = otpCode,
                ExpirationTime = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Passwordotps.Add(otpEntry);
            await _context.SaveChangesAsync();

            try
            {
                var senderEmail = _configuration["Smtp:EmailAddress"];
                var senderPassword = _configuration["Smtp:AppPassword"];

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail ?? string.Empty, "Project Management Admin"),
                    Subject = "Your Password Reset Code",
                    Body = $"Your one-time password for resetting your account is: <b>{otpCode}</b>. It will expire in 15 minutes.",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(user.Email);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Logs the full inner exception stack trace to the console window
                Console.WriteLine("===== SMTP ERROR: " + ex.ToString() + " =====");
                return StatusCode(500, new { Message = $"Failed to send email: {ex.Message}" });
            }

            return Ok(new { Message = "OTP sent successfully!" });
        }

        // POST: api/Users/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var otpEntry = await _context.Passwordotps
                .Where(o => o.Email == request.Email && o.OtpCode == request.OtpCode && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpEntry == null)
            {
                return BadRequest(new { Message = "Invalid OTP code." });
            }

            if (otpEntry.ExpirationTime < DateTime.UtcNow)
            {
                return BadRequest(new { Message = "This OTP has expired. Please request a new one." });
            }

            return Ok(new { Message = "OTP verified successfully." });
        }

        // POST: api/Users/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var otpEntry = await _context.Passwordotps
                .Where(o => o.Email == request.Email && o.OtpCode == request.OtpCode && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpEntry == null || otpEntry.ExpirationTime < DateTime.UtcNow)
            {
                return BadRequest(new { Message = "Invalid or expired OTP." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found." });
            }

            user.Password = request.NewPassword;
            otpEntry.IsUsed = true;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Password reset successfully." });
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}