using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumpenoWebassembly.Server.Migrations
{
    public partial class AddTotalJumps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalJumps",
                table: "UserStatistics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalJumps",
                table: "UserStatistics");
        }
    }
}
