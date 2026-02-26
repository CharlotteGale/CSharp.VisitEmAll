using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VisitEmAll.Migrations
{
    /// <inheritdoc />
    public partial class NewModelStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Holidays_HolidayId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Users_UserId",
                table: "Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activities",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Accommodation",
                table: "Holidays");

            migrationBuilder.RenameTable(
                name: "Activities",
                newName: "Activity");

            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "Holidays",
                newName: "TotalCost");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_UserId",
                table: "Activity",
                newName: "IX_Activity_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_HolidayId",
                table: "Activity",
                newName: "IX_Activity_HolidayId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Holidays",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activity",
                table: "Activity",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "HolidayDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HolidayId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HolidayDays_Holidays_HolidayId",
                        column: x => x.HolidayId,
                        principalTable: "Holidays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DayItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HolidayDayId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Item = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayItems_HolidayDays_HolidayDayId",
                        column: x => x.HolidayDayId,
                        principalTable: "HolidayDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayItems_HolidayDayId",
                table: "DayItems",
                column: "HolidayDayId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayDays_HolidayId",
                table: "HolidayDays",
                column: "HolidayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Holidays_HolidayId",
                table: "Activity",
                column: "HolidayId",
                principalTable: "Holidays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Users_UserId",
                table: "Activity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Holidays_HolidayId",
                table: "Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Users_UserId",
                table: "Activity");

            migrationBuilder.DropTable(
                name: "DayItems");

            migrationBuilder.DropTable(
                name: "HolidayDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activity",
                table: "Activity");

            migrationBuilder.RenameTable(
                name: "Activity",
                newName: "Activities");

            migrationBuilder.RenameColumn(
                name: "TotalCost",
                table: "Holidays",
                newName: "Cost");

            migrationBuilder.RenameIndex(
                name: "IX_Activity_UserId",
                table: "Activities",
                newName: "IX_Activities_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Activity_HolidayId",
                table: "Activities",
                newName: "IX_Activities_HolidayId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Holidays",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Accommodation",
                table: "Holidays",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activities",
                table: "Activities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Holidays_HolidayId",
                table: "Activities",
                column: "HolidayId",
                principalTable: "Holidays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Users_UserId",
                table: "Activities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
