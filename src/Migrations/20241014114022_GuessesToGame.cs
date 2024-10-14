using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Image_guesser.Migrations
{
    /// <inheritdoc />
    public partial class GuessesToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Sessions_SessionId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Guessers_Games_GameId",
                table: "Guessers");

            migrationBuilder.DropIndex(
                name: "IX_Guessers_GameId",
                table: "Guessers");

            migrationBuilder.DropIndex(
                name: "IX_Games_SessionId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Guessers");

            migrationBuilder.AddColumn<Guid>(
                name: "BaseGameId",
                table: "Guessers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Guess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GuessMessage = table.Column<string>(type: "TEXT", nullable: false),
                    NameOfGuesser = table.Column<string>(type: "TEXT", nullable: false),
                    TimeOfGuess = table.Column<string>(type: "TEXT", nullable: false),
                    BaseGameId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guess_Games_BaseGameId",
                        column: x => x.BaseGameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guessers_BaseGameId",
                table: "Guessers",
                column: "BaseGameId");

            migrationBuilder.CreateIndex(
                name: "IX_Guess_BaseGameId",
                table: "Guess",
                column: "BaseGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guessers_Games_BaseGameId",
                table: "Guessers",
                column: "BaseGameId",
                principalTable: "Games",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guessers_Games_BaseGameId",
                table: "Guessers");

            migrationBuilder.DropTable(
                name: "Guess");

            migrationBuilder.DropIndex(
                name: "IX_Guessers_BaseGameId",
                table: "Guessers");

            migrationBuilder.DropColumn(
                name: "BaseGameId",
                table: "Guessers");

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                table: "Guessers",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Guessers_GameId",
                table: "Guessers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_SessionId",
                table: "Games",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Sessions_SessionId",
                table: "Games",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guessers_Games_GameId",
                table: "Guessers",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
