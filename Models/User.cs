using System;
using System.ComponentModel.DataAnnotations;

namespace Modesta.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string PrivacySetting { get; set; } = "public";
        public string UITheme { get; set; } = "beige";
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

