using System.ComponentModel.DataAnnotations;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models
{
    public class User {
        public int Id { get; set; }
        [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(200)] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
