using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Security2.Web.Migrations
{
    public partial class AddTimePasswords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPasswords",
                columns: table => new
                {
                    UserGuid = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    IsUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswords", x => new { x.UserGuid, x.Date });
                    table.ForeignKey(
                        name: "FK_UserPasswords_Users_UserGuid",
                        column: x => x.UserGuid,
                        principalTable: "Users",
                        principalColumn: "UserGuid",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPasswords");
        }
    }
}
