using Microsoft.EntityFrameworkCore;

namespace NewsAppExample.Model
{
    public class NewsContext : DbContext
    {
        public NewsContext(DbContextOptions<NewsContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasOne(a => a.Author)
                      .WithMany(u => u.Articles)
                      .HasForeignKey(a => a.AuthorId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Article)
                      .WithMany(a => a.Comments)
                      .HasForeignKey(c => c.ArticleId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Role)
                      .WithMany()
                      .HasForeignKey(u => u.RoleId);

                entity.HasIndex(u => u.Username)
                      .IsUnique();
            });
        }
    }
}
