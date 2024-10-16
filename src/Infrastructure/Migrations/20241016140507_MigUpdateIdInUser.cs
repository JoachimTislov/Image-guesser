using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Image_guesser.Migrations
{
    /// <inheritdoc />
    public partial class MigUpdateIdInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomSizedImageTilesDirectoryId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomSizedImageTilesDirectoryId",
                table: "AspNetUsers");
        }
    }
}
