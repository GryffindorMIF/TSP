using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class ProductAd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductAd",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdImageUrl = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAd", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAd_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategory_CategoryId",
                table: "CategoryCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategory_ParentCategoryId",
                table: "CategoryCategory",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAd_ProductId",
                table: "ProductAd",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryCategory_Category_CategoryId",
                table: "CategoryCategory",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryCategory_Category_ParentCategoryId",
                table: "CategoryCategory",
                column: "ParentCategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryCategory_Category_CategoryId",
                table: "CategoryCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryCategory_Category_ParentCategoryId",
                table: "CategoryCategory");

            migrationBuilder.DropTable(
                name: "ProductAd");

            migrationBuilder.DropIndex(
                name: "IX_CategoryCategory_CategoryId",
                table: "CategoryCategory");

            migrationBuilder.DropIndex(
                name: "IX_CategoryCategory_ParentCategoryId",
                table: "CategoryCategory");
        }
    }
}
