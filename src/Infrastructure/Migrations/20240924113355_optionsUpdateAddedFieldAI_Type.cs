using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Image_guesser.Migrations
{
    /// <inheritdoc />
    public partial class optionsUpdateAddedFieldAI_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChosenImageName",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "ImageIdentifier",
                table: "Sessions",
                newName: "Options_ImageIdentifier");

            migrationBuilder.AddColumn<int>(
                name: "Options_AI_Type",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Options_AI_Type",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "Options_ImageIdentifier",
                table: "Sessions",
                newName: "ImageIdentifier");

            migrationBuilder.AddColumn<string>(
                name: "ChosenImageName",
                table: "Sessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
