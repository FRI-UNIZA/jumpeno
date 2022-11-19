using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumpenoWebassembly.Server.Migrations
{
    public partial class NotesForError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Errors",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Errors");
        }
    }
}
