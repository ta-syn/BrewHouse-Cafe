using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CafeManagement.Migrations
{
    /// <inheritdoc />
    public partial class CategorizedImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CafeTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TableNumber = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CafeTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    ImageEmoji = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ItemType = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    IsHot = table.Column<bool>(type: "boolean", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    CaffeineLevel = table.Column<int>(type: "integer", nullable: true),
                    Allergens = table.Column<string>(type: "text", nullable: true),
                    IsSeasonalItem = table.Column<bool>(type: "boolean", nullable: true),
                    Calories = table.Column<int>(type: "integer", nullable: true),
                    PreparationTimeMinutes = table.Column<int>(type: "integer", nullable: true),
                    IsVegetarian = table.Column<bool>(type: "boolean", nullable: true),
                    IsCrunchy = table.Column<bool>(type: "boolean", nullable: true),
                    PortionSize = table.Column<string>(type: "text", nullable: true),
                    LimitedEditionNote = table.Column<string>(type: "text", nullable: true),
                    AvailableUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountApplied = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    IsWalkIn = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                    ItemName = table.Column<string>(type: "text", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CafeTables",
                columns: new[] { "Id", "Capacity", "Status", "TableNumber" },
                values: new object[,]
                {
                    { 1, 2, 0, 1 },
                    { 2, 4, 0, 2 },
                    { 3, 2, 0, 3 },
                    { 4, 6, 0, 4 },
                    { 5, 4, 0, 5 }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "CaffeineLevel", "Category", "CreatedAt", "Description", "ImageEmoji", "ImageUrl", "IsAvailable", "IsHot", "ItemType", "Name", "Price", "Size" },
                values: new object[,]
                {
                    { 1, 95, 0, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4630), "Rich and bold single shot", "☕", "/images/menu/beverages/Espresso.png", true, true, "Beverage", "Espresso", 180m, "Medium" },
                    { 2, 150, 0, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4650), "Vanilla milk and espresso with caramel", "🥤", "/images/menu/beverages/Caramel Macchiato.png", true, true, "Beverage", "Caramel Macchiato", 320m, "Medium" },
                    { 3, 70, 0, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4650), "Premium grade Japanese matcha", "🍵", "/images/menu/beverages/Matcha Latte.png", true, true, "Beverage", "Matcha Latte", 350m, "Medium" },
                    { 4, 5, 0, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4650), "Creamy Belgian cocoa with marshmallows", "🍫", "/images/menu/beverages/Hot Chocolate.png", true, true, "Beverage", "Hot Chocolate", 280m, "Medium" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageEmoji", "ImageUrl", "IsAvailable", "IsVegetarian", "ItemType", "Name", "PreparationTimeMinutes", "Price" },
                values: new object[,]
                {
                    { 5, 1, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4670), "Sourdough bread with poached eggs", "🥑", "/images/menu/food/Avocado Toast.png", true, true, "Food", "Avocado Toast", 15, 450m },
                    { 6, 1, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4670), "Juicy chicken breast with house sauce", "🍔", "/images/menu/food/Grilled Chicken Burger.png", true, false, "Food", "Grilled Chicken Burger", 15, 380m },
                    { 7, 1, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4670), "Classic white sauce pasta with mushrooms", "🍝", "/images/menu/food/Pasta Alfredo.png", true, true, "Food", "Pasta Alfredo", 15, 550m },
                    { 8, 1, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4670), "Three-layered sandwich with turkey and egg", "🥪", "/images/menu/food/Club Sandwich.png", true, false, "Food", "Club Sandwich", 15, 420m }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Allergens", "Calories", "Category", "CreatedAt", "Description", "ImageEmoji", "ImageUrl", "IsAvailable", "IsSeasonalItem", "ItemType", "Name", "Price" },
                values: new object[,]
                {
                    { 9, "None", 450, 2, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4690), "Italian coffee-flavored dessert", "🍰", "/images/menu/desserts/Tiramisu.png", true, false, "Dessert", "Tiramisu", 400m },
                    { 10, "None", 520, 2, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4690), "New York style baked cheesecake", "🥧", "/images/menu/desserts/Blueberry Cheesecake.png", true, false, "Dessert", "Blueberry Cheesecake", 450m },
                    { 11, "None", 600, 2, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4690), "Warm fudge brownie with vanilla scoop", "🍨", "/images/menu/desserts/Brownie with Ice Cream.png", true, false, "Dessert", "Brownie with Ice Cream", 350m },
                    { 12, "None", 300, 2, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4690), "Box of 6 colorful French macarons", "🥯", "/images/menu/desserts/Macarons Box.png", true, false, "Dessert", "Macarons Box", 600m }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageEmoji", "ImageUrl", "IsAvailable", "IsCrunchy", "ItemType", "Name", "PortionSize", "Price" },
                values: new object[,]
                {
                    { 13, 3, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4710), "Salted golden fries with dip", "🍟", "/images/menu/snacks/Crispy French Fries.png", true, false, "Snack", "Crispy French Fries", "Regular", 220m },
                    { 14, 3, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4710), "Corn chips with cheese and salsa", "🌮", "/images/menu/snacks/Nachos Supreme.png", true, false, "Snack", "Nachos Supreme", "Shareable", 380m },
                    { 15, 3, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4710), "Beer-battered crispy onion rings", "🧅", "/images/menu/snacks/Onion Rings.png", true, false, "Snack", "Onion Rings", "Regular", 250m },
                    { 16, 3, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4720), "Spicy buffalo wings (6 pieces)", "🍗", "/images/menu/snacks/Chicken Wings.png", true, false, "Snack", "Chicken Wings", "Regular", 480m }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "AvailableUntil", "Category", "CreatedAt", "Description", "ImageEmoji", "ImageUrl", "IsAvailable", "ItemType", "LimitedEditionNote", "Name", "Price" },
                values: new object[,]
                {
                    { 17, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4730), "12-hour steeped artisan coffee", "🧊", "/images/menu/specials/Signature Cold Brew.png", true, "Special", "Summer Favorite", "Signature Cold Brew", 380m },
                    { 18, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4730), "Hand-tossed dough with premium toppings", "🍕", "/images/menu/specials/Chef's Pizza.png", true, "Special", "Limited Supply", "Chef's Pizza", 850m },
                    { 19, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4730), "Fragrant rose syrup with espresso", "🌹", "/images/menu/specials/Iced Rose Latte.png", true, "Special", "Exclusive", "Iced Rose Latte", 390m },
                    { 20, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, new DateTime(2026, 5, 2, 22, 10, 38, 536, DateTimeKind.Utc).AddTicks(4730), "Molten center matcha cake", "🍵", "/images/menu/specials/Matcha Lava Cake.png", true, "Special", "Spring Special", "Matcha Lava Cake", 450m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MenuItemId",
                table: "OrderItems",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CafeTables");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
