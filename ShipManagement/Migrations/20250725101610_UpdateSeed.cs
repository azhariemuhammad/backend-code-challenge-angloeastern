using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ports",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ports",
                columns: new[] { "Id", "Country", "CreatedAt", "Latitude", "Longitude", "Name" },
                values: new object[] { 2, "Singapore", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.29660000m, 3.80610000m, "Port of Singapore" });
        }
    }
}
