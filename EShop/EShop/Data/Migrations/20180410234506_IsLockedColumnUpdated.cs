using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class IsLockedColumnUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                "IsLocked",
                "Users",
                nullable: true,
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                "IsLocked",
                "Users",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}