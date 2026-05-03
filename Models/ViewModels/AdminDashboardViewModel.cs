using CafeManagement.Models;
using System.Collections.Generic;

namespace CafeManagement.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public int TotalItems { get; set; }
        public int TotalCustomers { get; set; }
        public List<PopularItemViewModel> PopularItems { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
    }
}
