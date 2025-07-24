using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipManagement.Migrations
{
    /// <inheritdoc />
    public partial class UseEfCoreManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserShips");

            migrationBuilder.CreateTable(
                name: "ShipUser",
                columns: table => new
                {
                    ShipsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipUser", x => new { x.ShipsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ShipUser_Ships_ShipsId",
                        column: x => x.ShipsId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipUser_UsersId",
                table: "ShipUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipUser");

            migrationBuilder.CreateTable(
                name: "UserShips",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ShipId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShips", x => new { x.UserId, x.ShipId });
                    table.ForeignKey(
                        name: "FK_UserShips_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserShips_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserShips_ShipId",
                table: "UserShips",
                column: "ShipId");
        }
    }
}
