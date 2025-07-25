
namespace ShipManagement.Tests.Services
{
    public class UserServiceTest
    {
        [Theory]
        [InlineData("TestUser", "Admin")]
        [InlineData("AnotherUser", "User")]
        public async Task CreateUserAsync_ReturnsResponse_WhenUserIsNew(string name, string role)
        {
            var options = new DbContextOptionsBuilder<ShipManagementContext>()
                .UseInMemoryDatabase(databaseName: $"CreateUser_{name}_{role}")
                .Options;

            using var context = new ShipManagementContext(options);
            var service = new UserService(context);
            var request = new UserDetailsRequest { Name = name, Role = role };

            var response = await service.CreateUserAsync(request);

            response.Should().NotBeNull();
            response.Id.Should().BeGreaterThan(0);
            var user = await context.Users.FindAsync(response.Id);
            user.Should().NotBeNull();
            user.Name.Should().Be(name);
            user.Role.Should().Be(role);
        }


        [Theory]
        [InlineData("ExistingUser", "Admin")]
        public async Task CreateUserAsync_ThrowsDuplicateUserNameException_WhenUserNameExists(string name, string role)
        {
            var options = new DbContextOptionsBuilder<ShipManagementContext>()
                .UseInMemoryDatabase(databaseName: $"CreateUser_Throws_{name}_{role}")
                .Options;

            using var context = new ShipManagementContext(options);
            var service = new UserService(context);
            var request = new UserDetailsRequest { Name = name, Role = role };

            await service.CreateUserAsync(request);

            Func<Task> act = async () => await service.CreateUserAsync(request);
            await act.Should().ThrowAsync<DuplicateUserNameException>();
        }
    }
}
