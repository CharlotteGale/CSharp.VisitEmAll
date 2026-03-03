using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitEmAll.Migrations
{
    /// <inheritdoc />
    public partial class YS_AUTHED_New_HeroImageUrl_Holiday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeroImageUrl",
                table: "Holidays",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeroImageUrl",
                table: "Holidays");
        }
    }
}
