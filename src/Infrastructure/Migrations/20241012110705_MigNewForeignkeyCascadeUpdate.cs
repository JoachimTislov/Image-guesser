using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Image_guesser.Migrations
{
    /// <inheritdoc />
    public partial class MigNewForeignkeyCascadeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Oracle_AI",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_User",
                table: "Oracles");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Oracle_AI",
                table: "Games",
                column: "AIOracleId",
                principalTable: "Oracles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Oracles_AspNetUsers_UserId",
                table: "Oracles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Oracle_AI",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Oracles_AspNetUsers_UserId",
                table: "Oracles");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Oracle_AI",
                table: "Games",
                column: "AIOracleId",
                principalTable: "Oracles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User",
                table: "Oracles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
