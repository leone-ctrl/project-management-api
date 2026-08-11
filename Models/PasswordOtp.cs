using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectmanagementAPI.Models
{
    public class PasswordOtp
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}