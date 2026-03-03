using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitEmAll.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLikedHolidayDbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLikedHoliday_Holidays_HolidayId",
                table: "UserLikedHoliday");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLikedHoliday_Users_UserId",
                table: "UserLikedHoliday");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLikedHoliday",
                table: "UserLikedHoliday");

            migrationBuilder.RenameTable(
                name: "UserLikedHoliday",
                newName: "UserLikedHolidays");

            migrationBuilder.RenameIndex(
                name: "IX_UserLikedHoliday_HolidayId",
                table: "UserLikedHolidays",
                newName: "IX_UserLikedHolidays_HolidayId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLikedHolidays",
                table: "UserLikedHolidays",
                columns: new[] { "UserId", "HolidayId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikedHolidays_Holidays_HolidayId",
                table: "UserLikedHolidays",
                column: "HolidayId",
                principalTable: "Holidays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikedHolidays_Users_UserId",
                table: "UserLikedHolidays",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLikedHolidays_Holidays_HolidayId",
                table: "UserLikedHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLikedHolidays_Users_UserId",
                table: "UserLikedHolidays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLikedHolidays",
                table: "UserLikedHolidays");

            migrationBuilder.RenameTable(
                name: "UserLikedHolidays",
                newName: "UserLikedHoliday");

            migrationBuilder.RenameIndex(
                name: "IX_UserLikedHolidays_HolidayId",
                table: "UserLikedHoliday",
                newName: "IX_UserLikedHoliday_HolidayId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLikedHoliday",
                table: "UserLikedHoliday",
                columns: new[] { "UserId", "HolidayId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikedHoliday_Holidays_HolidayId",
                table: "UserLikedHoliday",
                column: "HolidayId",
                principalTable: "Holidays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikedHoliday_Users_UserId",
                table: "UserLikedHoliday",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
