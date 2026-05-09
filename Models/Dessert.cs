namespace CafeManagement.Models
{
    public class Dessert : MenuItem {
        public string Allergens { get; set; } = "None";
        public bool IsSeasonalItem { get; set; } = false;
        public int Calories { get; set; } = 0;
        public bool ContainsNuts { get; set; }
        public bool IsGlutenFree { get; set; }

        public override string GetDescription() =>
            $"{Name}{(IsSeasonalItem ? " [Seasonal]" : "")} - {Description}. Allergens: {Allergens}";
    }
}
