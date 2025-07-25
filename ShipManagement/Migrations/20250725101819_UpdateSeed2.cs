using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ports",
                columns: new[] { "Id", "Country", "CreatedAt", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 4, "Indonesia", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), -6.12500000m, 106.88250000m, "Port of Tanjung Priok" },
                    { 5, "Singapore", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.26416700m, 103.82250000m, "Port of Singapore" },
                    { 6, "China", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 31.23000000m, 121.47370000m, "Port of Shanghai" },
                    { 7, "Hong Kong", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22.39642800m, 114.10950000m, "Port of Hong Kong" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ports",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ports",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ports",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ports",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
