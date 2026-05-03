namespace CafeManagement.Models
{
    // ═══ OOP CONCEPT: INHERITANCE — Beverage extends MenuItem ═══
    // ═══ OOP CONCEPT: POLYMORPHISM — override GetDescription() and ApplyDiscount() ═══
    public class Beverage : MenuItem {
        public bool IsHot { get; set; } = true;
        public string Size { get; set; } = "Medium"; // Small, Medium, Large
        public int CaffeineLevel { get; set; } = 0;

        public override string GetDescription() =>
            $"{Size} {(IsHot ? "Hot" : "Cold")} {Name} - {Description}";

        public override decimal ApplyDiscount(decimal percentage) {
            // Cold drinks get extra 5% discount — POLYMORPHISM
            try {
                if (percentage < 0 || percentage > 100)
                    throw new ArgumentException("Discount must be between 0 and 100.");
                decimal extra = IsHot ? 0 : 5;
                decimal totalDiscount = Math.Min(percentage + extra, 100);
                return Price - (Price * totalDiscount / 100);
            } catch (ArgumentException) {
                throw;
            }
        }
    }
}
