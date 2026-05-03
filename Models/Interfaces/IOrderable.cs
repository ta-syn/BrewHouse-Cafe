// ═══ OOP CONCEPT: INTERFACE ═══
// Defines contract that all orderable items must follow
namespace CafeManagement.Models.Interfaces
{
    public interface IOrderable {
        string Name { get; set; }
        decimal Price { get; set; }
        string GetDescription();
    }
}
