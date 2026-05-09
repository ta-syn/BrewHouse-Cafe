using CafeManagement.Models.Enums;

namespace CafeManagement.Models
{
    public class Snack : MenuItem
    {
        public bool IsCrunchy { get; set; }
        public string PortionSize { get; set; } = string.Empty; // e.g., "100g", "Small Bowl"
        public int SpiceLevel { get; set; } // 1 to 5

        public override string GetDescription() => $"{Description} (Portion: {PortionSize})";
    }
}
