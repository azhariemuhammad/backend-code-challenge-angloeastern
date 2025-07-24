using ShipManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ShipManagement.Data
{
    public class ShipManagementContext : DbContext
    {
        public DbSet<Ship> Ships { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Port> Ports { get; set; }
        public ShipManagementContext(DbContextOptions<ShipManagementContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ship>()
                .Property(s => s.Velocity)
                .HasPrecision(10, 2);
            modelBuilder.Entity<Ship>()
                .Property(s => s.Latitude)
                .HasPrecision(11, 8);
            modelBuilder.Entity<Ship>()
                .Property(s => s.Longitude)
                .HasPrecision(11, 8);
            modelBuilder.Entity<Port>()
                .Property(p => p.Latitude)
                .HasPrecision(11, 8);
            modelBuilder.Entity<Port>()
                .Property(p => p.Longitude)
                .HasPrecision(11, 8);
            modelBuilder.Entity<Ship>()
                .HasIndex(s => s.ShipCode)
                .IsUnique();
            modelBuilder.Entity<Ship>()
                .HasIndex(s => s.Name)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();
        }
    }
}