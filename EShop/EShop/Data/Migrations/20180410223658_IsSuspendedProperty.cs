using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class IsSuspendedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "FirstName",
                "Users");

            migrationBuilder.AddColumn<bool>(
                "IsSuspended",
                "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "IsSuspended",
                "Users");

            migrationBuilder.AddColumn<string>(
                "FirstName",
                "Users",
                nullable: true);
        }
    }
}