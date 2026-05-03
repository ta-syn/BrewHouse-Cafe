using CafeManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models
{
    public class CafeTable {
        public int Id { get; set; }
        [Range(1, 100)]
        public int TableNumber { get; set; }
        [Range(1, 20)]
        public int Capacity { get; set; } = 4;
        public TableStatus Status { get; set; } = TableStatus.Available;
    }
}
