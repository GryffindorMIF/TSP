using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class ProductImageAddingAndUserFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                "ShoppingCartId",
                "Users",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                "IsPrimary",
                "ProductImage",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "IsPrimary",
                "ProductImage");

            migrationBuilder.AlterColumn<int>(
                "ShoppingCartId",
                "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}