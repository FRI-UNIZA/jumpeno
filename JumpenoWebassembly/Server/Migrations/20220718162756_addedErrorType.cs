using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumpenoWebassembly.Server.Migrations
{
    public partial class addedErrorType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Errors",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Errors");
        }
    }
}
