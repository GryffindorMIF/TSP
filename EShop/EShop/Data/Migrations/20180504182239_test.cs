using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_ProductDiscount_ProductId",
                "ProductDiscount");

            migrationBuilder.AddUniqueConstraint(
                "AlternateKey_ProductId",
                "ProductDiscount",
                "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                "AlternateKey_ProductId",
                "ProductDiscount");

            migrationBuilder.CreateIndex(
                "IX_ProductDiscount_ProductId",
                "ProductDiscount",
                "ProductId",
                unique: true);
        }
    }
}