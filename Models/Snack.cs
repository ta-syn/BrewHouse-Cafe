using CafeManagement.Models.Enums;

namespace CafeManagement.Models
{
    public class Snack : MenuItem
    {
        public bool IsCrunchy { get; set; }
        public string PortionSize { get; set; } // e.g., "100g", "Small Bowl"

        public override string GetDescription() => $"{Description} (Portion: {PortionSize})";
    }
}
