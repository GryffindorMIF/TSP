using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class changetablename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                "PK_ProductDetails",
                "ProductDetails");

            migrationBuilder.RenameTable(
                "ProductDetails",
                newName: "ProductProperty");

            migrationBuilder.AddPrimaryKey(
                "PK_ProductProperty",
                "ProductProperty",
                "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                "PK_ProductProperty",
                "ProductProperty");

            migrationBuilder.RenameTable(
                "ProductProperty",
                newName: "ProductDetails");

            migrationBuilder.AddPrimaryKey(
                "PK_ProductDetails",
                "ProductDetails",
                "Id");
        }
    }
}