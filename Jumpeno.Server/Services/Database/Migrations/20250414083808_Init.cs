using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jumpeno.Server.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "VARCHAR(13)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Skin = table.Column<int>(type: "INT(1)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Activation",
                columns: table => new
                {
                    ID = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Activation_User_ID",
                        column: x => x.ID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Password",
                columns: table => new
                {
                    ID = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hash = table.Column<byte[]>(type: "BINARY(32)", nullable: false),
                    Salt = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Password", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Password_User_ID",
                        column: x => x.ID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Refresh",
                columns: table => new
                {
                    Token = table.Column<string>(type: "VARCHAR(512)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ID = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Origin = table.Column<string>(type: "VARCHAR(512)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expires = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refresh", x => x.Token);
                    table.ForeignKey(
                        name: "FK_Refresh_User_ID",
                        column: x => x.ID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Refresh_Expires",
                table: "Refresh",
                column: "Expires");

            migrationBuilder.CreateIndex(
                name: "IX_Refresh_ID",
                table: "Refresh",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_Refresh_Origin",
                table: "Refresh",
                column: "Origin");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedAt",
                table: "User",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Name",
                table: "User",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activation");

            migrationBuilder.DropTable(
                name: "Password");

            migrationBuilder.DropTable(
                name: "Refresh");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
