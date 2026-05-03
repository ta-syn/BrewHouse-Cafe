namespace CafeManagement.Models
{
    // ═══ OOP CONCEPT: INHERITANCE — Dessert extends MenuItem ═══
    // ═══ OOP CONCEPT: POLYMORPHISM — override GetDescription() and ApplyDiscount() ═══
    public class Dessert : MenuItem {
        public string Allergens { get; set; } = "None";
        public bool IsSeasonalItem { get; set; } = false;
        public int Calories { get; set; } = 0;

        public override string GetDescription() =>
            $"{Name}{(IsSeasonalItem ? " [Seasonal]" : "")} - {Description}. Allergens: {Allergens}";
    }
}
