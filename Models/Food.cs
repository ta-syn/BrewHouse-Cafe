namespace CafeManagement.Models
{
    // ═══ OOP CONCEPT: INHERITANCE — Food extends MenuItem ═══
    // ═══ OOP CONCEPT: POLYMORPHISM — override GetDescription() and ApplyDiscount() ═══
    public class Food : MenuItem {
        public int PreparationTimeMinutes { get; set; } = 15;
        public bool IsVegetarian { get; set; } = false;

        public override string GetDescription() =>
            $"{Name} ({(IsVegetarian ? "Veg" : "Non-Veg")}) - Ready in {PreparationTimeMinutes} mins. {Description}";
    }
}
