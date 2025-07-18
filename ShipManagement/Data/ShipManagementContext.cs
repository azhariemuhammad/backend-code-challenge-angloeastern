using ShipManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ShipManagement.Data
{
    public class ShipManagementContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Ship> Ships { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserShip> UserShips { get; set; }
        public DbSet<Port> Ports { get; set; }
        public ShipManagementContext(DbContextOptions<ShipManagementContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserShip>()
                .HasKey(us => new { us.UserId, us.ShipId });
            modelBuilder.Entity<UserShip>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserShips)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserShip>()
                .HasOne(us => us.Ship)
                .WithMany(s => s.UserShips)
                .HasForeignKey(us => us.ShipId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Ship>()
                .Property(s => s.Velocity)
                .HasPrecision(10, 2);
            modelBuilder.Entity<Ship>()
                .Property(s => s.Latitude)
                .HasPrecision(10, 8);
            modelBuilder.Entity<Ship>()
                .Property(s => s.Longitude)
                .HasPrecision(10, 8);
            modelBuilder.Entity<Port>()
                .Property(p => p.Latitude)
                .HasPrecision(10, 8);
            modelBuilder.Entity<Port>()
                .Property(p => p.Longitude)
                .HasPrecision(10, 8);
            modelBuilder.Entity<Ship>()
            .HasIndex(s => s.ShipCode)
            .IsUnique();
        }
    }
}