using LittleC.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleC.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<WasteReason> WasteReasons { get; set; }
        public DbSet<WasteLog> WasteLogs { get; set; }
        public DbSet<DailySummary> DailySummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PIN).IsRequired().HasMaxLength(4);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerPrice).HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<WasteReason>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<WasteLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalCost).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Shift).IsRequired().HasMaxLength(20);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.WasteLogs)
                      .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.MenuItem)
                      .WithMany(m => m.WasteLogs)
                      .HasForeignKey(e => e.MenuItemId);

                entity.HasOne(e => e.WasteReason)
                      .WithMany(r => r.WasteLogs)
                      .HasForeignKey(e => e.WasteReasonId);
            });

            modelBuilder.Entity<DailySummary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalWasteValue).HasColumnType("decimal(10,2)");
                entity.Property(e => e.PhotoUrl).HasMaxLength(500);

                entity.HasOne(e => e.SubmittedBy)
                      .WithMany()
                      .HasForeignKey(e => e.SubmittedByUserId);
            });

            // Seed waste reasons automatically
            modelBuilder.Entity<WasteReason>().HasData(
                new WasteReason { Id = 1, Reason = "Burnt" },
                new WasteReason { Id = 2, Reason = "Dropped" },
                new WasteReason { Id = 3, Reason = "Expired" },
                new WasteReason { Id = 4, Reason = "Wrong Order" },
                new WasteReason { Id = 5, Reason = "Overproduced" },
                new WasteReason { Id = 6, Reason = "Return" },
                new WasteReason { Id = 7, Reason = "Quality Issue" }
            );
        }
    }
}