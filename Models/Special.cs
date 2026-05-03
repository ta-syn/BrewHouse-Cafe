using CafeManagement.Models.Enums;

namespace CafeManagement.Models
{
    public class Special : MenuItem
    {
        public string LimitedEditionNote { get; set; }
        public DateTime AvailableUntil { get; set; }

        public override string GetDescription() => $"{Description} [Limited: {LimitedEditionNote}]";
    }
}
