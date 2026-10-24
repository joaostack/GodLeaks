using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using GodLeaks.Infrastructure.Persistence;

#nullable disable

namespace GodLeaks.Migrations;

[DbContext(typeof(GodLeaksDbContext))]
[Migration("20261024200000_CreateTables")]
public partial class CreateTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Admins",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                DiscordUserId = table.Column<string>(type: "TEXT", nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Admins", x => x.Id));

        migrationBuilder.CreateTable(
            name: "UserSubscriptions",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                DiscordUserId = table.Column<string>(type: "TEXT", nullable: false),
                PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_UserSubscriptions", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Admins_DiscordUserId",
            table: "Admins",
            column: "DiscordUserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserSubscriptions_DiscordUserId",
            table: "UserSubscriptions",
            column: "DiscordUserId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Admins");
        migrationBuilder.DropTable(name: "UserSubscriptions");
    }
}
