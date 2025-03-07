using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jumpeno.Server.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTrust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Trust",
                table: "Persons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3,
                column: "Trust",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Trust",
                table: "Persons");
        }
    }
}
