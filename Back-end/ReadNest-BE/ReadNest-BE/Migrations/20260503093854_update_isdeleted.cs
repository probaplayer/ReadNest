using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReadNest_BE.Migrations
{
    /// <inheritdoc />
    public partial class update_isdeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookmarks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    NovelId = table.Column<string>(type: "TEXT", nullable: true),
                    ChapterId = table.Column<string>(type: "TEXT", nullable: true),
                    ContentText = table.Column<string>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategorieNovels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    NovelId = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorieNovels", x => new { x.Id, x.CategoryId, x.NovelId });
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: true),
                    VolumnId = table.Column<string>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    P = table.Column<string>(type: "TEXT", nullable: true),
                    ImageId = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    ChapterId = table.Column<string>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageCleanupFailures",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ImageId = table.Column<string>(type: "TEXT", nullable: true),
                    ImageName = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    FirstAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageCleanupFailures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ImageName = table.Column<string>(type: "TEXT", nullable: true),
                    ImageSize = table.Column<long>(type: "INTEGER", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ContentType = table.Column<string>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Novels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Author = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    ImageId = table.Column<string>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Novels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    P = table.Column<string>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReadingHistorys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    NovelId = table.Column<string>(type: "TEXT", nullable: false),
                    VolumnId = table.Column<string>(type: "TEXT", nullable: false),
                    ChapterId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingHistorys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    NameRole = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.Id, x.UserId, x.RoleId });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    Token = table.Column<string>(type: "TEXT", nullable: true),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TokenExpiryTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Volumns",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ImageId = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: true),
                    NovelId = table.Column<string>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volumns", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreateBy", "CreateDate", "IsDeleted", "Name", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { "category-action", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Action", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-adapted-anime", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Adapted to Anime", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-adapted-drama", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Adapted to Drama", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-adapted-manga", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Adapted to Manga", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-adapted-manhua", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Adapted to Manhua", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-adapted-manhwa", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Adapted to Manhwa", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-adult", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Adult", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-adventure", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Adventure", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-age-gap", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Age Gap", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-boys-love", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Boys Love", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-character-growth", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Character Growth", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-chinese-novel", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Chinese Novel", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-comedy", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Comedy", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-cooking", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Cooking", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-different-social-status", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Different Social Status", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-drama", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Drama", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-ecchi", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Ecchi", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-english-novel", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "English Novel", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-fanfiction", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Fanfiction", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-fantasy", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Fantasy", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-female-protagonist", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Female Protagonist", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-game", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Game", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-gender-bender", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Gender Bender", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-harem", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Harem", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-historical", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Historical", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-horror", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Horror", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-incest", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Incest", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-isekai", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Isekai", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-josei", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Josei", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-korean-novel", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Korean Novel", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-magic", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Magic", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-martial-arts", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Martial Arts", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-mature", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Mature", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-mecha", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Mecha", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-military", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Military", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-misunderstanding", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Misunderstanding", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-mystery", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Mystery", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-netorare", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Netorare", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-one-shot", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "One shot", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-otome-game", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Otome Game", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-parody", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Parody", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-psychological", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Psychological", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-reverse-harem", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Reverse Harem", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-romance", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Romance", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-school-life", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "School Life", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-science-fiction", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Science Fiction", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-seinen", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Seinen", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-shoujo", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Shoujo", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-shoujo-ai", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Shoujo ai", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-shounen", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Shounen", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-shounen-ai", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Shounen ai", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-slice-of-life", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Slice of Life", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-slow-life", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Slow Life", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-sports", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Sports", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-super-power", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Super Power", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-supernatural", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Supernatural", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-suspense", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Suspense", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-tragedy", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Tragedy", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-wars", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Wars", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-web-novel", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Web Novel", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-workplace", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Workplace", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-yandere", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Yandere", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "category-yuri", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Yuri", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreateDate", "IsDeleted", "NameRole", "UpdateDate" },
                values: new object[,]
                {
                    { "role-admin-000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "role-author-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "role-reader-002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId", "CreateDate", "IsDeleted", "UpdateDate" },
                values: new object[] { "role-admin-132", "role-admin-000", "user-admin-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreateDate", "IsDeleted", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "Token", "TokenExpiryTime", "UpdateDate", "UserName" },
                values: new object[] { "user-admin-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "$2a$11$zwlXRWWdqbaAXS0uFIIv7e7ixOPRYLqyj05nnJdOCUSrBPrOVJdkO", null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Content_ChapterId_Order",
                table: "Contents",
                columns: new[] { "ChapterId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ReadingHistorys_CreateBy_NovelId",
                table: "ReadingHistorys",
                columns: new[] { "CreateBy", "NovelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookmarks");

            migrationBuilder.DropTable(
                name: "CategorieNovels");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "ImageCleanupFailures");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Novels");

            migrationBuilder.DropTable(
                name: "Prompts");

            migrationBuilder.DropTable(
                name: "ReadingHistorys");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Volumns");
        }
    }
}
