using CafeManagement.Models.Enums;
using CafeManagement.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models
{
    // ═══ OOP CONCEPT: ABSTRACT CLASS ═══
    // ═══ OOP CONCEPT: ENCAPSULATION — private backing fields ═══
    public abstract class MenuItem : IOrderable, IDiscountable {
        // Private backing fields — ENCAPSULATION
        private string _name = string.Empty;
        private decimal _price;

        public int Id { get; set; }

        [Required]
        public string Name {
            get => _name;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Item name cannot be empty.");
                _name = value;
            }
        }

        [Required]
        [Range(0.01, 99999)]
        public decimal Price {
            get => _price;
            set {
                if (value < 0)
                    throw new ArgumentException("Price cannot be negative.");
                _price = value;
            }
        }

        public string Category { get; set; } = "General";
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public string ImageEmoji { get; set; } = "☕";
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = "#6c757d"; // Default gray
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ABSTRACT METHOD — subclass must override
        public abstract string GetDescription();

        // VIRTUAL METHOD — subclass can override (POLYMORPHISM)
        public virtual decimal ApplyDiscount(decimal percentage) {
            // EXCEPTION HANDLING
            try {
                if (percentage < 0 || percentage > 100)
                    throw new ArgumentException("Discount must be between 0 and 100.");
                return Price - (Price * percentage / 100);
            } catch (ArgumentException) {
                throw;
            } catch (Exception ex) {
                throw new Exception($"Discount calculation failed: {ex.Message}");
            }
        }
    }
}
