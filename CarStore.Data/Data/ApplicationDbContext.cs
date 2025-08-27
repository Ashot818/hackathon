using CityLens.Data.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CityLens.Web.Data
{
    // Основной DbContext
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<PostVote> PostVotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Post
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
                entity.HasMany(p => p.Images)
                      .WithOne(i => i.Post)
                      .HasForeignKey(i => i.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(p => p.Votes)
                      .WithOne(v => v.Post)
                      .HasForeignKey(v => v.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PostImage
            modelBuilder.Entity<PostImage>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);
            });

            // PostVote
            modelBuilder.Entity<PostVote>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.VoteValue).IsRequired();
            });
        }
    }

    // Фабрика для design-time (миграции)
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=CityDb;Integrated Security=True;TrustServerCertificate=True;",
                sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
