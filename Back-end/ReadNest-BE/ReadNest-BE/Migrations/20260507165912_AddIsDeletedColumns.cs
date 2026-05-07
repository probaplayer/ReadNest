using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadNest_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserRoles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Volumns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Chapters",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Contents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Bookmarks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CategorieNovels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ReadingHistorys",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Prompts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Users");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Roles");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "UserRoles");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Volumns");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Chapters");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Contents");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Images");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Bookmarks");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Categories");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "CategorieNovels");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "ReadingHistorys");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Prompts");
        }
    }
}