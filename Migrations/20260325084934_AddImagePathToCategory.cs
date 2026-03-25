using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza_API.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_file_name",
                table: "categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_path",
                table: "categories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_file_name",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "image_path",
                table: "categories");
        }
    }
}
