using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class NestedCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_CategoryCategory_Category_CategoryId",
                "CategoryCategory");

            migrationBuilder.DropForeignKey(
                "FK_CategoryCategory_Category_ParentCategoryId",
                "CategoryCategory");

            migrationBuilder.DropIndex(
                "IX_CategoryCategory_CategoryId",
                "CategoryCategory");

            migrationBuilder.DropIndex(
                "IX_CategoryCategory_ParentCategoryId",
                "CategoryCategory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                "IX_CategoryCategory_CategoryId",
                "CategoryCategory",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_CategoryCategory_ParentCategoryId",
                "CategoryCategory",
                "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                "FK_CategoryCategory_Category_CategoryId",
                "CategoryCategory",
                "CategoryId",
                "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_CategoryCategory_Category_ParentCategoryId",
                "CategoryCategory",
                "ParentCategoryId",
                "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}