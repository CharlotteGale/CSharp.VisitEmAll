using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitEmAll.Migrations
{
    /// <inheritdoc />
    public partial class RemoveThumbnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Holidays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Holidays",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
