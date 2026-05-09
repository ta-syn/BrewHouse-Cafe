using Microsoft.EntityFrameworkCore;
using CafeManagement.Models;
using CafeManagement.Models.Enums;

namespace CafeManagement.Data
{
    public class CafeDbContext : DbContext
    {
        public CafeDbContext(DbContextOptions<CafeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Beverage> Beverages { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Dessert> Desserts { get; set; }
        public DbSet<Snack> Snacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CafeTable> CafeTables { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<RecipeItem> RecipeItems { get; set; }
        public DbSet<WastageLog> WastageLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CafeOutlet> Outlets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TPH Configuration
            modelBuilder.Entity<MenuItem>()
                .HasDiscriminator<string>("ItemType")
                .HasValue<Beverage>("Beverage")
                .HasValue<Food>("Food")
                .HasValue<Dessert>("Dessert")
                .HasValue<Snack>("Snack")
                .HasValue<Special>("Special");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.CompletedBy)
                .WithMany()
                .HasForeignKey(o => o.CompletedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            // 🔐 0. User Seeding (Loading from .env for Security)
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@cafe.com";
            var adminPass = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin123";
            var adminName = Environment.GetEnvironmentVariable("ADMIN_NAME") ?? "Admin";

            var staffEmail = Environment.GetEnvironmentVariable("STAFF_EMAIL") ?? "staff@cafe.com";
            var staffPass = Environment.GetEnvironmentVariable("STAFF_PASSWORD") ?? "staff123";
            var staffName = Environment.GetEnvironmentVariable("STAFF_NAME") ?? "Staff";

            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id = 1, 
                    Name = adminName, 
                    Email = adminEmail, 
                    Password = BCrypt.Net.BCrypt.HashPassword(adminPass), 
                    Role = UserRole.Admin,
                    IsEmailVerified = true
                },
                new User { 
                    Id = 2, 
                    Name = staffName, 
                    Email = staffEmail, 
                    Password = BCrypt.Net.BCrypt.HashPassword(staffPass), 
                    Role = UserRole.Staff,
                    IsEmailVerified = true
                }
            );

            // 🏠 1. Outlet Seeding (with Image)
            modelBuilder.Entity<CafeOutlet>().HasData(
                new CafeOutlet { 
                    Id = 1, 
                    Name = "BrewHouse Main (HQ)", 
                    Location = "Dhaka, BD", 
                    ContactNumber = "0123456789",
                    ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?q=80&w=2047&auto=format&fit=crop" 
                }
            );

            // ☕ 2. Beverages Seeding
            modelBuilder.Entity<Beverage>().HasData(
                new Beverage { Id = 1, Name = "Caramel Macchiato", Price = 350, Category = "Beverages", ImageUrl = "/images/menu/beverages/Caramel Macchiato.png", IsAvailable = true, IsHot = true, Size = "Medium", OutletId = 1, Description = "Rich espresso with buttery caramel syrup and velvety steamed milk." },
                new Beverage { Id = 2, Name = "Espresso", Price = 180, Category = "Beverages", ImageUrl = "/images/menu/beverages/Espresso.png", IsAvailable = true, IsHot = true, Size = "Small", OutletId = 1, Description = "A concentrated shot of pure coffee excellence with a rich crema." },
                new Beverage { Id = 3, Name = "Hot Chocolate", Price = 280, Category = "Beverages", ImageUrl = "/images/menu/beverages/Hot Chocolate.png", IsAvailable = true, IsHot = true, Size = "Medium", OutletId = 1, Description = "Smooth premium cocoa blended with hot milk and topped with marshmallows." },
                new Beverage { Id = 4, Name = "Matcha Latte", Price = 320, Category = "Beverages", ImageUrl = "/images/menu/beverages/Matcha Latte.png", IsAvailable = true, IsHot = true, Size = "Medium", OutletId = 1, Description = "Finely ground ceremonial grade Japanese green tea with steamed milk." }
            );

            // 🍰 3. Desserts Seeding
            modelBuilder.Entity<Dessert>().HasData(
                new Dessert { Id = 9, Name = "Blueberry Cheesecake", Price = 350, Category = "Desserts", ImageUrl = "/images/menu/desserts/Blueberry Cheesecake.png", IsAvailable = true, ContainsNuts = false, IsGlutenFree = false, OutletId = 1, Description = "Creamy New York style cheesecake topped with fresh blueberry compote." },
                new Dessert { Id = 10, Name = "Brownie with Ice Cream", Price = 280, Category = "Desserts", ImageUrl = "/images/menu/desserts/Brownie with Ice Cream.png", IsAvailable = true, ContainsNuts = true, IsGlutenFree = false, OutletId = 1, Description = "Warm, fudgy chocolate brownie served with a scoop of vanilla bean ice cream." },
                new Dessert { Id = 11, Name = "Macarons Box", Price = 450, Category = "Desserts", ImageUrl = "/images/menu/desserts/Macarons Box.png", IsAvailable = true, ContainsNuts = true, IsGlutenFree = true, OutletId = 1, Description = "Assorted box of 6 handcrafted delicate French macarons." },
                new Dessert { Id = 12, Name = "Tiramisu", Price = 320, Category = "Desserts", ImageUrl = "/images/menu/desserts/Tiramisu.png", IsAvailable = true, ContainsNuts = false, IsGlutenFree = false, OutletId = 1, Description = "Traditional Italian dessert with coffee-soaked ladyfingers and mascarpone." }
            );

            // 🍔 4. Food Seeding
            modelBuilder.Entity<Food>().HasData(
                new Food { Id = 5, Name = "Avocado Toast", Price = 450, Category = "Food", ImageUrl = "/images/menu/food/Avocado Toast.png", IsAvailable = true, PreparationTimeMinutes = 10, IsVegetarian = true, OutletId = 1, Description = "Fresh mashed avocado on toasted artisanal sourdough with chili flakes." },
                new Food { Id = 6, Name = "Club Sandwich", Price = 380, Category = "Food", ImageUrl = "/images/menu/food/Club Sandwich.png", IsAvailable = true, PreparationTimeMinutes = 15, IsVegetarian = false, OutletId = 1, Description = "Classic triple-decker with grilled chicken, egg, bacon, and crisp lettuce." },
                new Food { Id = 7, Name = "Grilled Chicken Burger", Price = 420, Category = "Food", ImageUrl = "/images/menu/food/Grilled Chicken Burger.png", IsAvailable = true, PreparationTimeMinutes = 20, IsVegetarian = false, OutletId = 1, Description = "Juicy flame-grilled chicken breast with caramelized onions and spicy mayo." },
                new Food { Id = 8, Name = "Pasta Alfredo", Price = 480, Category = "Food", ImageUrl = "/images/menu/food/Pasta Alfredo.png", IsAvailable = true, PreparationTimeMinutes = 15, IsVegetarian = true, OutletId = 1, Description = "Fettuccine pasta tossed in a rich, creamy parmesan and garlic sauce." }
            );

            // 🍟 5. Snacks Seeding
            modelBuilder.Entity<Snack>().HasData(
                new Snack { Id = 13, Name = "Chicken Wings", Price = 350, Category = "Snacks", ImageUrl = "/images/menu/snacks/Chicken Wings.png", IsAvailable = true, PortionSize = "6 Pieces", SpiceLevel = 3, OutletId = 1, Description = "Crispy fried wings tossed in our signature spicy buffalo sauce." },
                new Snack { Id = 14, Name = "Crispy French Fries", Price = 180, Category = "Snacks", ImageUrl = "/images/menu/snacks/Crispy French Fries.png", IsAvailable = true, PortionSize = "Large", SpiceLevel = 1, OutletId = 1, Description = "Golden, hand-cut fries seasoned with sea salt and rosemary." },
                new Snack { Id = 15, Name = "Nachos Supreme", Price = 420, Category = "Snacks", ImageUrl = "/images/menu/snacks/Nachos Supreme.png", IsAvailable = true, PortionSize = "Regular", SpiceLevel = 2, OutletId = 1, Description = "Loaded corn tortillas with cheese sauce, jalapeños, and fresh salsa." },
                new Snack { Id = 16, Name = "Onion Rings", Price = 220, Category = "Snacks", ImageUrl = "/images/menu/snacks/Onion Rings.png", IsAvailable = true, PortionSize = "Large", SpiceLevel = 1, OutletId = 1, Description = "Crispy beer-battered onion rings served with a tangy garlic dip." }
            );

            // ✨ 6. Specials Seeding
            modelBuilder.Entity<Special>().HasData(
                new Special { Id = 17, Name = "Chef's Pizza", Price = 850, Category = "Specials", ImageUrl = "/images/menu/specials/Chef's Pizza.png", IsAvailable = true, OutletId = 1, Description = "Artisan thin-crust pizza with premium toppings curated by our head chef." },
                new Special { Id = 18, Name = "Iced Rose Latte", Price = 380, Category = "Specials", ImageUrl = "/images/menu/specials/Iced Rose Latte.png", IsAvailable = true, OutletId = 1, Description = "Refreshing cold latte infused with natural rose extract and honey." },
                new Special { Id = 19, Name = "Matcha Lava Cake", Price = 420, Category = "Specials", ImageUrl = "/images/menu/specials/Matcha Lava Cake.png", IsAvailable = true, OutletId = 1, Description = "Molten center matcha sponge cake served with black sesame crumble." },
                new Special { Id = 20, Name = "Signature Cold Brew", Price = 350, Category = "Specials", ImageUrl = "/images/menu/specials/Signature Cold Brew.png", IsAvailable = true, OutletId = 1, Description = "18-hour slow-steeped coffee for a smooth and bold caffeine kick." }
            );

            // 🪑 7. Tables Seeding
            modelBuilder.Entity<CafeTable>().HasData(
                new CafeTable { Id = 1, TableNumber = 1, Capacity = 2, Status = TableStatus.Available, OutletId = 1 },
                new CafeTable { Id = 2, TableNumber = 2, Capacity = 4, Status = TableStatus.Available, OutletId = 1 },
                new CafeTable { Id = 3, TableNumber = 3, Capacity = 4, Status = TableStatus.Available, OutletId = 1 },
                new CafeTable { Id = 4, TableNumber = 4, Capacity = 6, Status = TableStatus.Available, OutletId = 1 }
            );

            // 📦 8. Inventory Seeding (with Images)
            modelBuilder.Entity<InventoryItem>().HasData(
                new InventoryItem { 
                    Id = 1, Name = "Espresso Beans", StockQuantity = 5000, Unit = "g", MinStockLevel = 1000, OutletId = 1, LastUpdated = DateTime.UtcNow, UnitCost = 2.5m,
                    ImageUrl = "https://images.unsplash.com/photo-1559056199-641a0ac8b55e?q=80&w=2070&auto=format&fit=crop" 
                },
                new InventoryItem { 
                    Id = 2, Name = "Whole Milk", StockQuantity = 20000, Unit = "ml", MinStockLevel = 5000, OutletId = 1, LastUpdated = DateTime.UtcNow, UnitCost = 0.1m,
                    ImageUrl = "https://images.unsplash.com/photo-1550583724-125581cc255b?q=80&w=1974&auto=format&fit=crop"
                },
                new InventoryItem { 
                    Id = 3, Name = "Chicken Breast", StockQuantity = 10000, Unit = "g", MinStockLevel = 2000, OutletId = 1, LastUpdated = DateTime.UtcNow, UnitCost = 0.5m,
                    ImageUrl = "https://images.unsplash.com/photo-1604503468506-a8da13d82791?q=80&w=1970&auto=format&fit=crop"
                }
            );
        }
    }
}
