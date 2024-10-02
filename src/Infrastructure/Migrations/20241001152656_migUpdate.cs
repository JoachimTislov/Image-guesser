using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Image_guesser.Migrations
{
    /// <inheritdoc />
    public partial class migUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");
        }
    }
}
