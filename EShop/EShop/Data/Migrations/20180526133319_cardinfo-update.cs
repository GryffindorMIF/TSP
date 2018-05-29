using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class cardinfoupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_CardInfo_Users_UserId",
                "CardInfo");

            migrationBuilder.DropIndex(
                "IX_CardInfo_UserId",
                "CardInfo");

            migrationBuilder.AlterColumn<string>(
                "UserId",
                "CardInfo",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                "ExpYear",
                "CardInfo",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                "ExpMonth",
                "CardInfo",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "UserId",
                "CardInfo",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                "ExpYear",
                "CardInfo",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                "ExpMonth",
                "CardInfo",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                "IX_CardInfo_UserId",
                "CardInfo",
                "UserId");

            migrationBuilder.AddForeignKey(
                "FK_CardInfo_Users_UserId",
                "CardInfo",
                "UserId",
                "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}