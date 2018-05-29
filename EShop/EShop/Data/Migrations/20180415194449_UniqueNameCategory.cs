using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class UniqueNameCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "Name",
                "Category",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                "IX_Category_Name",
                "Category",
                "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_Category_Name",
                "Category");

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Category",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}