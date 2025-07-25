
namespace ShipManagement.Tests.Controller
{
    public class UserControllerTest
    {
        [Theory, AutoMoqData]
        public async Task GetUsersAsync_ReturnsOk(
            List<GetUserResponse> response,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.GetUsersAsync()).ReturnsAsync(response);
            var controller = new UserController(mockService.Object);

            var result = await controller.GetUsersAsync();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeOfType<List<GetUserResponse>>();
            ((List<GetUserResponse>)okResult.Value).Count.Should().Be(response.Count);
        }

        [Theory, AutoMoqData]
        public async Task CreateUserAsync_ReturnsOk(
            CreateUserResponse response,
            UserDetailsRequest request,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.CreateUserAsync(request)).ReturnsAsync(response);
            var controller = new UserController(mockService.Object);

            var result = await controller.CreateUserAsync(request);
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeOfType<CreateUserResponse>();
            ((CreateUserResponse)okResult.Value).Id.Should().Be(response.Id);
        }

        [Theory, AutoMoqData]
        public async Task GetUserById_ReturnsOk_WhenUserExists(
            int id,
            GetUserResponse userResponse,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(userResponse);
            var controller = new UserController(mockService.Object);

            var result = await controller.GetUserByIdAsync(id);
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeOfType<GetUserResponse>();
            ((GetUserResponse)okResult.Value).Id.Should().Be(userResponse.Id);
        }

        [Theory, AutoMoqData]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist(
            int id,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync((GetUserResponse?)null);
            var controller = new UserController(mockService.Object);

            var result = await controller.GetUserByIdAsync(id);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoMoqData]
        public async Task DeleteUser_ReturnsNoContent_WhenUserDeleted(
            int id,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(true);
            var controller = new UserController(mockService.Object);

            var result = await controller.DeleteUser(id);
            result.Should().BeOfType<NoContentResult>();
        }

        [Theory, AutoMoqData]
        public async Task DeleteUser_ReturnsNotFound_WhenUserNotFound(
            int id,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(false);
            var controller = new UserController(mockService.Object);

            var result = await controller.DeleteUser(id);
            result.Should().BeOfType<NotFoundResult>();
        }


        [Theory, AutoMoqData]
        public async Task AssignShipsToUserSync_ReturnsSuccess_WhenShipsAssigned(
            int id,
            List<string> shipCodes,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.AssignShipsToUserSync(id, shipCodes)).ReturnsAsync((GetUserResponse?)null);
            var controller = new UserController(mockService.Object);

            var result = await controller.AssignShipsToUserSync(id, shipCodes);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().NotBeNull();
            var messageProperty = okResult.Value.GetType().GetProperty("message");
            messageProperty.Should().NotBeNull();

            var messageValue = messageProperty?.GetValue(okResult.Value);
            messageValue.Should().Be("User's ships assigned successfully.");
        }

        [Theory, AutoMoqData]
        public async Task UnassignShipsFromUserAsync_ReturnSuccessMessage(
            int id,
            List<string> shipCodes,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.UnassignShipsFromUserAsync(id, shipCodes)).Returns(Task.CompletedTask);
            var controller = new UserController(mockService.Object);

            var result = await controller.UnassignShipsFromUserAsync(id, shipCodes);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            var messageProperty = okResult.Value.GetType().GetProperty("message");
            messageProperty.Should().NotBeNull();

            var messageValue = messageProperty?.GetValue(okResult.Value);
            messageValue.Should().Be("User's ships unassigned successfully.");
        }

        [Theory, AutoMoqData]
        public async Task AssignShipsToUserSync_ReturnsError_WhenUserIsNotFound(
            List<string> shipCodes,
            [Frozen] Mock<IUserService> mockService)
        {
            var notFoundId = 1111;
            var errorMessage = string.Format(Constants.Messages.UserShip.NOT_ASSIGNED, notFoundId, shipCodes.FirstOrDefault());
            mockService.Setup(s => s.AssignShipsToUserSync(notFoundId, shipCodes)).ThrowsAsync(new KeyNotFoundException(errorMessage));
            var controller = new UserController(mockService.Object);

            var error = await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.AssignShipsToUserSync(notFoundId, shipCodes));

            error.Message.Should().Be(errorMessage);
        }
    }
}
