using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeManagement.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCategoryToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "MenuItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Beverage", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3220) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Beverage", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3240) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Beverage", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3240) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Beverage", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3240) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Food", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3260) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Food", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3260) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Food", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3270) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Food", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3270) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Dessert", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3280) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Dessert", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3280) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Dessert", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3290) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Dessert", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3290) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Snack", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3310) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Snack", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3310) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Snack", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3310) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Snack", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3310) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Special", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3330) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Special", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3330) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Special", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3330) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { "Special", new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3330) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "MenuItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 0, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2110) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 0, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2120) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 0, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2130) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 0, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2130) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 1, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2140) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 1, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2150) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 1, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2150) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 1, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2150) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 2, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2170) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 2, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2170) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 2, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2170) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 2, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2180) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 3, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2190) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 3, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2190) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 3, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2190) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 3, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2200) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 4, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2210) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 4, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2210) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 4, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2220) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Category", "CreatedAt" },
                values: new object[] { 4, new DateTime(2026, 5, 2, 22, 57, 32, 656, DateTimeKind.Utc).AddTicks(2220) });
        }
    }
}
