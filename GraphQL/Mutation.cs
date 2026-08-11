using System.Net;
using System.Net.Mail;
using Dapper;
using Microsoft.Data.SqlClient;
using ProjectmanagementAPI.Models;
using System.Data;

namespace ProjectmanagementAPI
{
    public class Mutation
    {
        // 1. LOGIN MUTATION
        [GraphQLName("login")]
        public async Task<User> Login(
            [Service] IConfiguration config,
            string email,
            string password)
        {
            using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));

            string sql = "SELECT UserId, FullName, Email, Role FROM USERS WHERE Email = @Email AND Password = @Password";
            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email, Password = password });

            if (user == null)
            {
                throw new Exception("Invalid email or password.");
            }

            return user;
        }

        // ... [Keep your other methods (AddUser, UpdateTaskStatus, DeleteProject, BulkUpdateUserStatus) as they are] ...

        // 6. REQUEST OTP FOR PASSWORD RESET
        [GraphQLName("requestPasswordOtp")]
        public async Task<bool> RequestPasswordOtp(
            [Service] IConfiguration config,
            string email)
        {
            using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));

            // Check if user exists in your USERS table
            string checkUserSql = "SELECT COUNT(1) FROM USERS WHERE Email = @Email";
            bool userExists = await connection.ExecuteScalarAsync<bool>(checkUserSql, new { Email = email });

            if (!userExists)
            {
                // Fail silently for security so attackers don't know valid emails
                return true;
            }

            // Generate a random 6-digit OTP code
            string otpCode = new Random().Next(100000, 999999).ToString();
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(10); // Valid for 10 minutes

            // Insert OTP record into DB
            string insertOtpSql = @"
                INSERT INTO PasswordOtps (Email, OtpCode, ExpirationTime) 
                VALUES (@Email, @OtpCode, @ExpirationTime)";

            await connection.ExecuteAsync(insertOtpSql, new { Email = email, OtpCode = otpCode, ExpirationTime = expirationTime });

            // Send real email via Gmail SMTP
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(
                        config["Smtp:EmailAddress"],
                        config["Smtp:AppPassword"]
                    ),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(config["Smtp:EmailAddress"], "Project Management App"),
                    Subject = "Your Password Reset OTP Code",
                    Body = $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                            <h2>Password Reset Request</h2>
                            <p>Your one-time verification code is:</p>
                            <h1 style='color: #4f46e5; letter-spacing: 5px;'>{otpCode}</h1>
                            <p>This code will expire in <strong>10 minutes</strong>.</p>
                        </div>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMTP ERROR] Failed to send email: {ex.Message}");
                throw new Exception("Unable to send OTP email at this time.");
            }
        }

        // 7. VERIFY OTP AND RESET PASSWORD
        [GraphQLName("resetPasswordWithOtp")]
        public async Task<bool> ResetPasswordWithOtp(
            [Service] IConfiguration config,
            string email,
            string otpCode,
            string newPassword)
        {
            using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));

            // Find valid active OTP record
            string checkOtpSql = @"
                SELECT TOP 1 Id 
                FROM PasswordOtps 
                WHERE Email = @Email 
                  AND OtpCode = @OtpCode 
                  AND IsUsed = 0 
                  AND ExpirationTime > GETUTCDATE()
                ORDER BY CreatedAt DESC";

            int? otpId = await connection.QueryFirstOrDefaultAsync<int?>(checkOtpSql, new { Email = email, OtpCode = otpCode });

            if (otpId == null)
            {
                throw new Exception("Invalid or expired OTP code.");
            }

            // Update the user's password
            string updatePasswordSql = "UPDATE USERS SET Password = @Password WHERE Email = @Email";
            await connection.ExecuteAsync(updatePasswordSql, new { Password = newPassword, Email = email });

            // Mark OTP as used
            string markOtpUsedSql = "UPDATE PasswordOtps SET IsUsed = 1 WHERE Id = @Id";
            await connection.ExecuteAsync(markOtpUsedSql, new { Id = otpId.Value });

            return true;
        }
    }
}