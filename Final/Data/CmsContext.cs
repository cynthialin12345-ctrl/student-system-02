using Microsoft.EntityFrameworkCore;
using Final.Models;

namespace Final.Data
{
    public class CmsContext : DbContext
    {
        public CmsContext(DbContextOptions<CmsContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Subcategory> Subcategory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Activity>().ToTable("Activity");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Subcategory>().ToTable("Subcategory");

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 設定外鍵關聯
            modelBuilder.Entity<Activity>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Category)
                .WithMany()
                .HasForeignKey(a => a.CateID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Subcategory)
                .WithMany()
                .HasForeignKey(a => a.SubID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subcategory>()
                .HasOne(s => s.Category)
                .WithMany()
                .HasForeignKey(s => s.CateID)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
