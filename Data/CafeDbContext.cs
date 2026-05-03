using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using CafeManagement.Models;
using CafeManagement.Models.Enums;

namespace CafeManagement.Data
{
    public class CafeDbContext : DbContext, IDataProtectionKeyContext
    {
        public CafeDbContext(DbContextOptions<CafeDbContext> options) : base(options) { }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Beverage> Beverages { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Dessert> Desserts { get; set; }
        public DbSet<Snack> Snacks { get; set; }
        public DbSet<Special> Specials { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CafeTable> CafeTables { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MenuItem>()
                .HasDiscriminator<string>("ItemType")
                .HasValue<Beverage>("Beverage")
                .HasValue<Food>("Food")
                .HasValue<Dessert>("Dessert")
                .HasValue<Snack>("Snack")
                .HasValue<Special>("Special");

            modelBuilder.Entity<CafeTable>().HasData(
                new CafeTable { Id = 1, TableNumber = 1, Capacity = 2, Status = TableStatus.Available },
                new CafeTable { Id = 2, TableNumber = 2, Capacity = 4, Status = TableStatus.Available },
                new CafeTable { Id = 3, TableNumber = 3, Capacity = 2, Status = TableStatus.Available },
                new CafeTable { Id = 4, TableNumber = 4, Capacity = 6, Status = TableStatus.Available },
                new CafeTable { Id = 5, TableNumber = 5, Capacity = 4, Status = TableStatus.Available }
            );

            // ═══ CATEGORIZED CUSTOMER IMAGES ═══
            
            // Beverages
            modelBuilder.Entity<Beverage>().HasData(
                new Beverage { Id = 1, Name = "Espresso", Price = 180, Category = "Beverage", ImageEmoji = "☕", ImageUrl = "/images/menu/beverages/Espresso.png", Description = "Rich and bold single shot", CaffeineLevel = 95 },
                new Beverage { Id = 2, Name = "Caramel Macchiato", Price = 320, Category = "Beverage", ImageEmoji = "🥤", ImageUrl = "/images/menu/beverages/Caramel Macchiato.png", Description = "Vanilla milk and espresso with caramel", CaffeineLevel = 150 },
                new Beverage { Id = 3, Name = "Matcha Latte", Price = 350, Category = "Beverage", ImageEmoji = "🍵", ImageUrl = "/images/menu/beverages/Matcha Latte.png", Description = "Premium grade Japanese matcha", CaffeineLevel = 70 },
                new Beverage { Id = 4, Name = "Hot Chocolate", Price = 280, Category = "Beverage", ImageEmoji = "🍫", ImageUrl = "/images/menu/beverages/Hot Chocolate.png", Description = "Creamy Belgian cocoa with marshmallows", CaffeineLevel = 5 }
            );

            // Foods
            modelBuilder.Entity<Food>().HasData(
                new Food { Id = 5, Name = "Avocado Toast", Price = 450, Category = "Food", ImageEmoji = "🥑", ImageUrl = "/images/menu/food/Avocado Toast.png", Description = "Sourdough bread with poached eggs", IsVegetarian = true },
                new Food { Id = 6, Name = "Grilled Chicken Burger", Price = 380, Category = "Food", ImageEmoji = "🍔", ImageUrl = "/images/menu/food/Grilled Chicken Burger.png", Description = "Juicy chicken breast with house sauce", IsVegetarian = false },
                new Food { Id = 7, Name = "Pasta Alfredo", Price = 550, Category = "Food", ImageEmoji = "🍝", ImageUrl = "/images/menu/food/Pasta Alfredo.png", Description = "Classic white sauce pasta with mushrooms", IsVegetarian = true },
                new Food { Id = 8, Name = "Club Sandwich", Price = 420, Category = "Food", ImageEmoji = "🥪", ImageUrl = "/images/menu/food/Club Sandwich.png", Description = "Three-layered sandwich with turkey and egg", IsVegetarian = false }
            );

            // Desserts
            modelBuilder.Entity<Dessert>().HasData(
                new Dessert { Id = 9, Name = "Tiramisu", Price = 400, Category = "Dessert", ImageEmoji = "🍰", ImageUrl = "/images/menu/desserts/Tiramisu.png", Description = "Italian coffee-flavored dessert", Calories = 450 },
                new Dessert { Id = 10, Name = "Blueberry Cheesecake", Price = 450, Category = "Dessert", ImageEmoji = "🥧", ImageUrl = "/images/menu/desserts/Blueberry Cheesecake.png", Description = "New York style baked cheesecake", Calories = 520 },
                new Dessert { Id = 11, Name = "Brownie with Ice Cream", Price = 350, Category = "Dessert", ImageEmoji = "🍨", ImageUrl = "/images/menu/desserts/Brownie with Ice Cream.png", Description = "Warm fudge brownie with vanilla scoop", Calories = 600 },
                new Dessert { Id = 12, Name = "Macarons Box", Price = 600, Category = "Dessert", ImageEmoji = "🥯", ImageUrl = "/images/menu/desserts/Macarons Box.png", Description = "Box of 6 colorful French macarons", Calories = 300 }
            );

            // Snacks
            modelBuilder.Entity<Snack>().HasData(
                new Snack { Id = 13, Name = "Crispy French Fries", Price = 220, Category = "Snack", ImageEmoji = "🍟", ImageUrl = "/images/menu/snacks/Crispy French Fries.png", Description = "Salted golden fries with dip", PortionSize = "Regular" },
                new Snack { Id = 14, Name = "Nachos Supreme", Price = 380, Category = "Snack", ImageEmoji = "🌮", ImageUrl = "/images/menu/snacks/Nachos Supreme.png", Description = "Corn chips with cheese and salsa", PortionSize = "Shareable" },
                new Snack { Id = 15, Name = "Onion Rings", Price = 250, Category = "Snack", ImageEmoji = "🧅", ImageUrl = "/images/menu/snacks/Onion Rings.png", Description = "Beer-battered crispy onion rings", PortionSize = "Regular" },
                new Snack { Id = 16, Name = "Chicken Wings", Price = 480, Category = "Snack", ImageEmoji = "🍗", ImageUrl = "/images/menu/snacks/Chicken Wings.png", Description = "Spicy buffalo wings (6 pieces)", PortionSize = "Regular" }
            );

            // Specials
            modelBuilder.Entity<Special>().HasData(
                new Special { Id = 17, Name = "Signature Cold Brew", Price = 380, Category = "Special", ImageEmoji = "🧊", ImageUrl = "/images/menu/specials/Signature Cold Brew.png", Description = "12-hour steeped artisan coffee", LimitedEditionNote = "Summer Favorite" },
                new Special { Id = 18, Name = "Chef's Pizza", Price = 850, Category = "Special", ImageEmoji = "🍕", ImageUrl = "/images/menu/specials/Chef's Pizza.png", Description = "Hand-tossed dough with premium toppings", LimitedEditionNote = "Limited Supply" },
                new Special { Id = 19, Name = "Iced Rose Latte", Price = 390, Category = "Special", ImageEmoji = "🌹", ImageUrl = "/images/menu/specials/Iced Rose Latte.png", Description = "Fragrant rose syrup with espresso", LimitedEditionNote = "Exclusive" },
                new Special { Id = 20, Name = "Matcha Lava Cake", Price = 450, Category = "Special", ImageEmoji = "🍵", ImageUrl = "/images/menu/specials/Matcha Lava Cake.png", Description = "Molten center matcha cake", LimitedEditionNote = "Spring Special" }
            );
        }
    }
}
