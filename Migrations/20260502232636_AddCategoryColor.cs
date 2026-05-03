using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryColor",
                table: "MenuItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1390) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1400) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1410) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1410) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1430) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1430) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1430) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1430) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1450) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1450) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1450) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1450) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1470) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1470) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1470) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1470) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1490) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1490) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1490) });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CategoryColor", "CreatedAt" },
                values: new object[] { "#6c757d", new DateTime(2026, 5, 2, 23, 26, 36, 396, DateTimeKind.Utc).AddTicks(1490) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryColor",
                table: "MenuItems");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3220));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3240));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3240));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3240));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3260));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3260));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3270));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3270));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3280));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3280));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3290));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3290));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3310));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3310));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3310));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3310));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3330));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3330));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3330));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 2, 23, 13, 48, 68, DateTimeKind.Utc).AddTicks(3330));
        }
    }
}
