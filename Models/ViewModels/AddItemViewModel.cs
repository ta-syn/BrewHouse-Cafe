using System.ComponentModel.DataAnnotations;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models.ViewModels
{
    public class AddItemViewModel {
        [Required] public string ItemType { get; set; } = "Beverage";
        public string? NewItemType { get; set; } // For "Add New" option
        [Required] public string Name { get; set; } = string.Empty;
        [Range(0, 99999)] public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "Beverage";
        public string? NewCategory { get; set; } // For "Add New" option
        public string CategoryColor { get; set; } = "#6c757d";
        public string ImageEmoji { get; set; } = "☕";
        public IFormFile? ImageFile { get; set; } // For actual file upload
        public bool IsAvailable { get; set; } = true;
        
        // Beverage fields
        public bool IsHot { get; set; } = true;
        public string Size { get; set; } = "Medium";
        
        // Food fields
        public int PreparationTimeMinutes { get; set; } = 10;
        public bool IsVegetarian { get; set; } = false;
        
        // Dessert fields
        public string Allergens { get; set; } = "None";
        public bool IsSeasonalItem { get; set; } = false;
    }
}
