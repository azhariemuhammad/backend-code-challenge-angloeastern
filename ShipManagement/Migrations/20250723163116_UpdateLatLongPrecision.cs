using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLatLongPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Ships",
                type: "numeric(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,8)",
                oldPrecision: 10,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Ships",
                type: "numeric(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,8)",
                oldPrecision: 10,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Ports",
                type: "numeric(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,8)",
                oldPrecision: 10,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Ports",
                type: "numeric(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,8)",
                oldPrecision: 10,
                oldScale: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Ships",
                type: "numeric(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Ships",
                type: "numeric(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Ports",
                type: "numeric(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Ports",
                type: "numeric(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(11,8)",
                oldPrecision: 11,
                oldScale: 8);
        }
    }
}
