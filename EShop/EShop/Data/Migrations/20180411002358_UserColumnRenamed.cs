using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class UserColumnRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "IsLocked",
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

            migrationBuilder.AddColumn<bool>(
                "IsLocked",
                "Users",
                nullable: true);
        }
    }
}