using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumpenoWebassembly.Server.Migrations
{
    public partial class UserGameTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GameTime",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartGameTime",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartGameTime",
                table: "Users");
        }
    }
}
