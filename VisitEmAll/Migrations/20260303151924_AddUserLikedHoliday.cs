using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitEmAll.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLikedHoliday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLikedHoliday",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    HolidayId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikedHoliday", x => new { x.UserId, x.HolidayId });
                    table.ForeignKey(
                        name: "FK_UserLikedHoliday_Holidays_HolidayId",
                        column: x => x.HolidayId,
                        principalTable: "Holidays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLikedHoliday_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLikedHoliday_HolidayId",
                table: "UserLikedHoliday",
                column: "HolidayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLikedHoliday");
        }
    }
}
