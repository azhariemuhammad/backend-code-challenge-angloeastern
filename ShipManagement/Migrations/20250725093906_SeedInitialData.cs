using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipManagement.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ports",
                columns: new[] { "Id", "Country", "CreatedAt", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "Netherlands", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(890), 51.92250000m, 4.47917000m, "Port of Rotterdam" },
                    { 2, "Singapore", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(1020), 1.29660000m, 3.80610000m, "Port of Singapore" },
                    { 3, "United States", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(1030), 33.73650000m, -18.29230000m, "Port of Los Angeles" }
                });

            migrationBuilder.InsertData(
                table: "Ships",
                columns: new[] { "Id", "CreatedAt", "Latitude", "Longitude", "Name", "ShipCode", "UpdatedAt", "Velocity" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(6060), 1.3521m, 103.8198m, "Aurora", "AE-001", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(6170), 22.5m },
                    { 2, new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(6290), 35.6895m, 139.6917m, "Endeavour", "AE-002", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(6290), 18.7m },
                    { 3, new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(6290), 51.5074m, -0.1278m, "Odyssey", "AE-003", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(6290), 25.0m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Name", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(4680), "Alice", "Admin", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(4800) },
                    { 2, new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(4920), "Bob", "Operator", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(4920) },
                    { 3, new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(4930), "Charlie", "Manager", new DateTime(2025, 7, 25, 9, 39, 6, 171, DateTimeKind.Utc).AddTicks(4930) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ports",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ports",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ports",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ships",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ships",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ships",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
