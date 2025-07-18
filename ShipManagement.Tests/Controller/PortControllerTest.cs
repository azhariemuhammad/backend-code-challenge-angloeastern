using Moq;
using ShipManagement.Interfaces;
using ShipManagement.Controllers;
using ShipManagement.Models.DTOs;
using ShipManagement.Models;
using ShipManagement.Constants;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ShipManagement.Tests.Controller
{
    public class PortControllerTest
    {
        private readonly PortController controller;
        private readonly Mock<IPortService> _mockService;

        public PortControllerTest()
        {
            _mockService = new Mock<IPortService>();
            controller = new PortController(_mockService.Object);
        }

        [Fact]
        public async Task GetClosestPort_ShouldReturnOkResult_WhenPortExists()
        {
            // Arrange
            var shipId = 1;
            var port = new Port { Id = 2, Name = "Port A", Country = "Country", Latitude = 1, Longitude = 2 };
            var dto = new PortWithDistanceDto { Port = port, DistanceKm = 100 };
            _mockService.Setup(s => s.GetClosestPortAsync(shipId)).ReturnsAsync(dto);

            // Act
            var result = await controller.GetClosestPort(shipId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<PortWithDistanceDto>(okResult.Value);
            Assert.Equal(dto.DistanceKm, returnedDto.DistanceKm);
            Assert.Equal(dto.Port.Name, returnedDto.Port.Name);
        }

        [Fact]
        public async Task GetClosestPort_ShouldReturnNotFound_WhenPortDoesNotExist()
        {
            // Arrange
            var shipId = 1;
            _mockService.Setup(s => s.GetClosestPortAsync(shipId)).ReturnsAsync((PortWithDistanceDto?)null);

            // Act
            var result = await controller.GetClosestPort(shipId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var errorObj = notFound.Value;
            var type = errorObj.GetType();
            Assert.Equal(string.Format(Messages.Port.ShipNotFoundForPort, shipId), type.GetProperty("message")?.GetValue(errorObj)?.ToString());
        }

        [Fact]
        public async Task GetClosestPort_ShouldReturnBadRequest_OnArgumentException()
        {
            // Arrange
            var shipId = 1;
            _mockService.Setup(s => s.GetClosestPortAsync(shipId)).ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act
            var result = await controller.GetClosestPort(shipId);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorObj = badRequest.Value;
            var type = errorObj.GetType();
            Assert.Equal("Invalid argument", type.GetProperty("message")?.GetValue(errorObj)?.ToString());
        }

        [Fact]
        public async Task GetClosestPort_ShouldReturnNotFound_OnInvalidOperationException()
        {
            // Arrange
            var shipId = 1;
            _mockService.Setup(s => s.GetClosestPortAsync(shipId)).ThrowsAsync(new InvalidOperationException("Not found"));

            // Act
            var result = await controller.GetClosestPort(shipId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var errorObj = notFound.Value;
            var type = errorObj.GetType();
            Assert.Equal("Not found", type.GetProperty("message")?.GetValue(errorObj)?.ToString());
        }

        [Fact]
        public async Task GetClosestPort_ShouldReturnStatus500_OnException()
        {
            // Arrange
            var shipId = 1;
            _mockService.Setup(s => s.GetClosestPortAsync(shipId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await controller.GetClosestPort(shipId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
            var errorObj = statusResult.Value;
            var type = errorObj.GetType();
            Assert.Equal(Messages.Port.ClosestPortError, type.GetProperty("message")?.GetValue(errorObj)?.ToString());
            Assert.Equal("Test exception", type.GetProperty("error")?.GetValue(errorObj)?.ToString());
        }
    }
}
