using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models
{
    public class Discount {
        public int Id { get; set; }
        [Required][MaxLength(50)] public string Code { get; set; } = string.Empty;
        [Range(0, 100)] public decimal Percentage { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddYears(1);
    }
}
