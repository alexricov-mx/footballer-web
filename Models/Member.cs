using System.ComponentModel.DataAnnotations;

namespace FootballerWeb.Models
{
    public class Member
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? ProfilePictureUrl { get; set; }
        
        [Required]
        public string Provider { get; set; } = string.Empty; // "Microsoft" o "Google"
        
        [Required]
        public string ProviderId { get; set; } = string.Empty; // ID del proveedor
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        public string? RefreshToken { get; set; }
        
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}