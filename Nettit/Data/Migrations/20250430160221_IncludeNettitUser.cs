using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nettit.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncludeNettitUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NettitUserId",
                table: "Chatrooms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Chatrooms_NettitUserId",
                table: "Chatrooms",
                column: "NettitUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chatrooms_AspNetUsers_NettitUserId",
                table: "Chatrooms",
                column: "NettitUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chatrooms_AspNetUsers_NettitUserId",
                table: "Chatrooms");

            migrationBuilder.DropIndex(
                name: "IX_Chatrooms_NettitUserId",
                table: "Chatrooms");

            migrationBuilder.DropColumn(
                name: "NettitUserId",
                table: "Chatrooms");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
