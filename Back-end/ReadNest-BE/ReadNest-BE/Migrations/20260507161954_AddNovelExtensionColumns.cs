using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadNest_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddNovelExtensionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdult",
                table: "Novels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Novels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SharedUserIds",
                table: "Novels",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdult",
                table: "Novels");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Novels");

            migrationBuilder.DropColumn(
                name: "SharedUserIds",
                table: "Novels");
        }
    }
}
