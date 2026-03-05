using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitEmAll.Migrations
{
    /// <inheritdoc />
    public partial class LatestDBMigrationWithAddedCountriesAndImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HolidayImage_Holidays_HolidayId",
                table: "HolidayImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HolidayImage",
                table: "HolidayImage");

            migrationBuilder.RenameTable(
                name: "HolidayImage",
                newName: "HolidayImages");

            migrationBuilder.RenameIndex(
                name: "IX_HolidayImage_HolidayId",
                table: "HolidayImages",
                newName: "IX_HolidayImages_HolidayId");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Holidays",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HolidayImages",
                table: "HolidayImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HolidayImages_Holidays_HolidayId",
                table: "HolidayImages",
                column: "HolidayId",
                principalTable: "Holidays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HolidayImages_Holidays_HolidayId",
                table: "HolidayImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HolidayImages",
                table: "HolidayImages");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Holidays");

            migrationBuilder.RenameTable(
                name: "HolidayImages",
                newName: "HolidayImage");

            migrationBuilder.RenameIndex(
                name: "IX_HolidayImages_HolidayId",
                table: "HolidayImage",
                newName: "IX_HolidayImage_HolidayId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HolidayImage",
                table: "HolidayImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HolidayImage_Holidays_HolidayId",
                table: "HolidayImage",
                column: "HolidayId",
                principalTable: "Holidays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
