using System.ComponentModel.DataAnnotations;
using CafeManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Models
{
    public class CafeTable
    {
        public int Id { get; set; }

        // 🏢 Phase 5: Multi-Outlet Support
        public int? OutletId { get; set; }
        public CafeOutlet? Outlet { get; set; }

        [Required]
        public int TableNumber { get; set; }

        [Required]
        public int Capacity { get; set; }

        public TableStatus Status { get; set; } = TableStatus.Available;
        
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
