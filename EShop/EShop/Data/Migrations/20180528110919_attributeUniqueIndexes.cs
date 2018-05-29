using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class attributeUniqueIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_Product_Name",
                "Product");

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Product",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Description",
                "Product",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "AttributeValue",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Attribute",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                "IX_Product_Name",
                "Product",
                "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_AttributeValue_Name",
                "AttributeValue",
                "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                "IX_Attribute_Name",
                "Attribute",
                "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_Product_Name",
                "Product");

            migrationBuilder.DropIndex(
                "IX_AttributeValue_Name",
                "AttributeValue");

            migrationBuilder.DropIndex(
                "IX_Attribute_Name",
                "Attribute");

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Product",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                "Description",
                "Product",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "AttributeValue",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Attribute",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                "IX_Product_Name",
                "Product",
                "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}