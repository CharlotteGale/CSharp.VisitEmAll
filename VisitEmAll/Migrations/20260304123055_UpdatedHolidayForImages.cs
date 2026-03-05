using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VisitEmAll.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedHolidayForImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HolidayImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    HolidayId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HolidayImage_Holidays_HolidayId",
                        column: x => x.HolidayId,
                        principalTable: "Holidays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HolidayImage_HolidayId",
                table: "HolidayImage",
                column: "HolidayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HolidayImage");
        }
    }
}
