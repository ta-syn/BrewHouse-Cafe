namespace CafeManagement.Models.ViewModels
{
    public class PopularItemViewModel {
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ImageEmoji { get; set; } = "☕";
        public string? ImageUrl { get; set; }
        public int TotalOrdered { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
