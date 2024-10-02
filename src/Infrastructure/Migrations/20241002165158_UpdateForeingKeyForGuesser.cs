using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Image_guesser.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeingKeyForGuesser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guessers_Games_BaseGameId",
                table: "Guessers");

            migrationBuilder.DropIndex(
                name: "IX_Guessers_BaseGameId",
                table: "Guessers");

            migrationBuilder.DropColumn(
                name: "BaseGameId",
                table: "Guessers");

            migrationBuilder.CreateIndex(
                name: "IX_Guessers_GameId",
                table: "Guessers",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guessers_Games_GameId",
                table: "Guessers",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guessers_Games_GameId",
                table: "Guessers");

            migrationBuilder.DropIndex(
                name: "IX_Guessers_GameId",
                table: "Guessers");

            migrationBuilder.AddColumn<Guid>(
                name: "BaseGameId",
                table: "Guessers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guessers_BaseGameId",
                table: "Guessers",
                column: "BaseGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guessers_Games_BaseGameId",
                table: "Guessers",
                column: "BaseGameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
