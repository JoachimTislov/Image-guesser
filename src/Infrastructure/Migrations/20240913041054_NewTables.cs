using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Image_guesser.Infrastructure
{
    /// <inheritdoc />
    public partial class NewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OracleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameMode = table.Column<string>(type: "TEXT", nullable: false),
                    NumberOfGames = table.Column<int>(type: "INTEGER", nullable: false),
                    OracleIsAI = table.Column<bool>(type: "INTEGER", nullable: false),
                    GameStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Timer = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", nullable: false),
                    Link = table.Column<string>(type: "TEXT", nullable: false),
                    FolderWithImagePiecesLink = table.Column<string>(type: "TEXT", nullable: false),
                    PieceCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Oracles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalGuesses = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfTilesRevealed = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageIdentifier = table.Column<string>(type: "TEXT", nullable: false),
                    OracleType = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    RandomNumbersAI = table.Column<string>(type: "TEXT", nullable: true),
                    UserInfo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oracles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionHostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChosenOracle = table.Column<Guid>(type: "TEXT", nullable: false),
                    Options_NumberOfRounds = table.Column<int>(type: "INTEGER", nullable: false),
                    Options_LobbySize = table.Column<int>(type: "INTEGER", nullable: false),
                    Options_GameMode = table.Column<int>(type: "INTEGER", nullable: false),
                    Options_RandomPictureMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    Options_RandomOracle = table.Column<bool>(type: "INTEGER", nullable: false),
                    Options_UseAI = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImageIdentifier = table.Column<string>(type: "TEXT", nullable: false),
                    ChosenImageName = table.Column<string>(type: "TEXT", nullable: false),
                    SessionStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guessers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeSpan = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Guesses = table.Column<int>(type: "INTEGER", nullable: false),
                    WrongGuessCounter = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guessers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guessers_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SessionId",
                table: "AspNetUsers",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Guessers_GameId",
                table: "Guessers",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sessions_SessionId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Guessers");

            migrationBuilder.DropTable(
                name: "ImageRecords");

            migrationBuilder.DropTable(
                name: "Oracles");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SessionId",
                table: "AspNetUsers");
        }
    }
}
