using Microsoft.EntityFrameworkCore;
using ReadNest_Enums;
using ReadNest_Models;

namespace ReadNest_BE.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Volumn> Volumns { get; set; }
        public DbSet<ReadingHistory> ReadingHistorys { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryNovel> CategorieNovels { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Prompt> Prompts { get; set; }
        public DbSet<Novel> Novels { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<ImageCleanupFailure> ImageCleanupFailures { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ReadingHistory>()
                .HasIndex(r => new { r.CreateBy, r.NovelId })
                .IsUnique();

            modelBuilder.Entity<Content>()
                .HasIndex(c => new { c.ChapterId, c.Order })
                .HasDatabaseName("IX_Content_ChapterId_Order");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.UserName).IsUnique();
                entity.Property(u => u.UserName)
                    .HasMaxLength(256)
                    .IsRequired();
            });

            modelBuilder.Entity<CategoryNovel>()
                .HasKey(x => new { x.Id, x.CategoryId, x.NovelId });
            modelBuilder.Entity<UserRole>()
                .HasKey(x => new { x.Id, x.UserId, x.RoleId });

            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var roles = Enum.GetValues(typeof(RoleType))
                .Cast<RoleType>()
                .Select(r => new Role
                {
                    Id = $"role-{r.ToString().ToLower()}-{(int)r:D3}",
                    NameRole = r,
                    CreateDate = seedDate,
                    UpdateDate = seedDate
                })
                .ToList();

            var userAdminId = "user-admin-001";
            var roleAdminId = roles.First(r => r.NameRole == RoleType.ADMIN).Id;

            var users = new List<User>
            {
                new User
                {
                    Id = userAdminId,
                    UserName = "admin",
                    PasswordHash = "$2a$11$zwlXRWWdqbaAXS0uFIIv7e7ixOPRYLqyj05nnJdOCUSrBPrOVJdkO",
                    CreateDate = seedDate,
                    UpdateDate = seedDate,
                }
            };

            modelBuilder.Entity<Role>().HasData(roles);
            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                Id = "role-admin-132",
                RoleId = roleAdminId,
                UserId = userAdminId,
                CreateDate = seedDate,
                UpdateDate = seedDate
            });

            var listCategories = new List<Category>()
            {
                new Category { Id = "category-action", Name = "Action", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-adapted-anime", Name = "Adapted to Anime", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-adapted-drama", Name = "Adapted to Drama", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-adapted-manga", Name = "Adapted to Manga", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-adapted-manhwa", Name = "Adapted to Manhwa", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-adapted-manhua", Name = "Adapted to Manhua", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-adult", Name = "Adult", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-adventure", Name = "Adventure", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-age-gap", Name = "Age Gap", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-boys-love", Name = "Boys Love", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-character-growth", Name = "Character Growth", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-chinese-novel", Name = "Chinese Novel", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-comedy", Name = "Comedy", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-cooking", Name = "Cooking", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-different-social-status", Name = "Different Social Status", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-drama", Name = "Drama", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-ecchi", Name = "Ecchi", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-english-novel", Name = "English Novel", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-fanfiction", Name = "Fanfiction", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-fantasy", Name = "Fantasy", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-female-protagonist", Name = "Female Protagonist", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-game", Name = "Game", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-gender-bender", Name = "Gender Bender", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-harem", Name = "Harem", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-historical", Name = "Historical", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-horror", Name = "Horror", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-incest", Name = "Incest", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-isekai", Name = "Isekai", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-josei", Name = "Josei", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-korean-novel", Name = "Korean Novel", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-magic", Name = "Magic", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-martial-arts", Name = "Martial Arts", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-mature", Name = "Mature", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-mecha", Name = "Mecha", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-military", Name = "Military", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-misunderstanding", Name = "Misunderstanding", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-mystery", Name = "Mystery", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-netorare", Name = "Netorare", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-one-shot", Name = "One shot", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-otome-game", Name = "Otome Game", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-parody", Name = "Parody", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-psychological", Name = "Psychological", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-reverse-harem", Name = "Reverse Harem", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-romance", Name = "Romance", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-school-life", Name = "School Life", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-science-fiction", Name = "Science Fiction", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-seinen", Name = "Seinen", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-shoujo", Name = "Shoujo", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-shoujo-ai", Name = "Shoujo ai", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-shounen", Name = "Shounen", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-shounen-ai", Name = "Shounen ai", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-slice-of-life", Name = "Slice of Life", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-slow-life", Name = "Slow Life", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-sports", Name = "Sports", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-super-power", Name = "Super Power", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-supernatural", Name = "Supernatural", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-suspense", Name = "Suspense", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-tragedy", Name = "Tragedy", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-wars", Name = "Wars", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-web-novel", Name = "Web Novel", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-workplace", Name = "Workplace", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-yandere", Name = "Yandere", CreateDate = seedDate, UpdateDate = seedDate },
                new Category { Id = "category-yuri", Name = "Yuri", CreateDate = seedDate, UpdateDate = seedDate },
            };

            modelBuilder.Entity<Category>().HasData(listCategories);
        }
    }
}