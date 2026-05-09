using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = "Info"; // Info, Warning, Success, Danger

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(6);

        public string? TargetUrl { get; set; } // Link to relevant page (e.g., Inventory)
    }
}
