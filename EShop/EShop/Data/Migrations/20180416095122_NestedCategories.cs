using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class NestedCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryCategory_Category_CategoryId",
                table: "CategoryCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryCategory_Category_ParentCategoryId",
                table: "CategoryCategory");

            migrationBuilder.DropIndex(
                name: "IX_CategoryCategory_CategoryId",
                table: "CategoryCategory");

            migrationBuilder.DropIndex(
                name: "IX_CategoryCategory_ParentCategoryId",
                table: "CategoryCategory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategory_CategoryId",
                table: "CategoryCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategory_ParentCategoryId",
                table: "CategoryCategory",
                column: "ParentCategoryId");

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
    }
}
