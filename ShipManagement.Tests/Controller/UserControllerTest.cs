using Moq;
using ShipManagement.Controllers;
using ShipManagement.Interfaces;
using ShipManagement.Models.Request;
using ShipManagement.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using AutoFixture.Xunit2;

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
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.IsType<List<GetUserResponse>>(okResult.Value);
            Assert.Equal(response.Count, ((List<GetUserResponse>)okResult.Value).Count);
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
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.IsType<CreateUserResponse>(okResult.Value);
            Assert.Equal(response.Id, ((CreateUserResponse)okResult.Value).Id);
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
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.IsType<GetUserResponse>(okResult.Value);
            Assert.Equal(userResponse.Id, ((GetUserResponse)okResult.Value).Id);
        }

        [Theory, AutoMoqData]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist(
            int id,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync((GetUserResponse?)null);
            var controller = new UserController(mockService.Object);

            var result = await controller.GetUserByIdAsync(id);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Theory, AutoMoqData]
        public async Task DeleteUser_ReturnsNoContent_WhenUserDeleted(
            int id,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(true);
            var controller = new UserController(mockService.Object);

            var result = await controller.DeleteUser(id);
            Assert.IsType<NoContentResult>(result);
        }

        [Theory, AutoMoqData]
        public async Task DeleteUser_ReturnsNotFound_WhenUserNotFound(
            int id,
            [Frozen] Mock<IUserService> mockService)
        {
            mockService.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(false);
            var controller = new UserController(mockService.Object);

            var result = await controller.DeleteUser(id);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
