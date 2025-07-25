namespace ShipManagement.Tests.Services
{
    public class ShipServiceTest
    {
        [Fact]
        public async Task CreateShipAsync_ReturnsResponse_WhenShipIsNew()
        {
            var options = new DbContextOptionsBuilder<ShipManagementContext>()
                .UseInMemoryDatabase(databaseName: "CreateShipAsync_ReturnsResponse_WhenShipIsNew")
                .Options;

            using var context = new ShipManagementContext(options);
            var service = new ShipService(context);
            var request = new CreateShipRequest
            {
                ShipCode = "AE-001",
                Name = "Test Ship",
                Velocity = 20,
                Latitude = 10.5M,
                Longitude = 20.5M
            };

            var response = await service.CreateShipAsync(request);

            response.Should().NotBeNull();
            response.Id.Should().BeGreaterThan(0);
            response.ShipCode.Should().Be(request.ShipCode);
            response.Name.Should().Be(request.Name);
            response.Velocity.Should().Be(request.Velocity);
            response.Latitude.Should().Be(request.Latitude);
            response.Longitude.Should().Be(request.Longitude);

            var ship = await context.Ships.FindAsync(response.Id);
            ship.Should().NotBeNull();
            ship.ShipCode.Should().Be(request.ShipCode);
            ship.Name.Should().Be(request.Name);
        }

        [Fact]
        public async Task AssignUsersToShipAsync_ThrowsKeyNotFoundException_WhenNoUsersFound()
        {
            var options = new DbContextOptionsBuilder<ShipManagementContext>()
                .UseInMemoryDatabase(databaseName: "AssignUsersToShipAsync_ThrowsKeyNotFoundException")
                .Options;

            using var context = new ShipManagementContext(options);
            var service = new ShipService(context);
            var userIds = new List<int> { 1, 2, 3 };
            var shipCode = "SHIP001";

            Func<Task> act = async () => await service.AssignUsersToShipAsync(userIds, shipCode);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
