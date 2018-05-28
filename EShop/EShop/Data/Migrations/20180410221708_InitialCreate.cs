using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "UserNameIndex",
                "AspNetUsers");

            migrationBuilder.DropIndex(
                "IX_AspNetUserRoles_UserId",
                "AspNetUserRoles");

            migrationBuilder.DropIndex(
                "RoleNameIndex",
                "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                "FirstName",
                "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "AspNetUsers",
                "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                "FK_AspNetUserTokens_AspNetUsers_UserId",
                "AspNetUserTokens",
                "UserId",
                "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_AspNetUserTokens_AspNetUsers_UserId",
                "AspNetUserTokens");

            migrationBuilder.DropIndex(
                "UserNameIndex",
                "AspNetUsers");

            migrationBuilder.DropIndex(
                "RoleNameIndex",
                "AspNetRoles");

            migrationBuilder.DropColumn(
                "FirstName",
                "AspNetUsers");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "AspNetUsers",
                "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_UserId",
                "AspNetUserRoles",
                "UserId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName");
        }
    }
}