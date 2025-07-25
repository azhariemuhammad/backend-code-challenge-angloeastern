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

            // seed initial data
            var staticDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Port>().HasData(
                new Port { Id = 1, Name = "Port of Rotterdam", Country = "Netherlands", Latitude = 51.92250000M, Longitude = 4.47917000M, CreatedAt = staticDate },
                new Port { Id = 3, Name = "Port of Los Angeles", Country = "United States", Latitude = 33.73650000M, Longitude = -18.29230000M, CreatedAt = staticDate },
                new Port { Id = 4, Name = "Port of Tanjung Priok", Country = "Indonesia", Latitude = -6.12500000M, Longitude = 106.88250000M, CreatedAt = staticDate },
                new Port { Id = 5, Name = "Port of Singapore", Country = "Singapore", Latitude = 1.26416700M, Longitude = 103.82250000M, CreatedAt = staticDate },
                new Port { Id = 6, Name = "Port of Shanghai", Country = "China", Latitude = 31.23000000M, Longitude = 121.47370000M, CreatedAt = staticDate },
                new Port { Id = 7, Name = "Port of Hong Kong", Country = "Hong Kong", Latitude = 22.39642800M, Longitude = 114.10950000M, CreatedAt = staticDate }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Alice", Role = "Admin", CreatedAt = staticDate, UpdatedAt = staticDate },
                new User { Id = 2, Name = "Bob", Role = "Operator", CreatedAt = staticDate, UpdatedAt = staticDate },
                new User { Id = 3, Name = "Charlie", Role = "Manager", CreatedAt = staticDate, UpdatedAt = staticDate }
            );

            modelBuilder.Entity<Ship>().HasData(
                new Ship { Id = 1, ShipCode = "AE-001", Name = "Aurora", Velocity = 22.5M, Latitude = 1.3521M, Longitude = 103.8198M, CreatedAt = staticDate, UpdatedAt = staticDate },
                new Ship { Id = 2, ShipCode = "AE-002", Name = "Endeavour", Velocity = 18.7M, Latitude = 35.6895M, Longitude = 139.6917M, CreatedAt = staticDate, UpdatedAt = staticDate },
                new Ship { Id = 3, ShipCode = "AE-003", Name = "Odyssey", Velocity = 25.0M, Latitude = 51.5074M, Longitude = -0.1278M, CreatedAt = staticDate, UpdatedAt = staticDate }
            );
        }
    }
}