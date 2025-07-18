using Moq;
using ShipManagement.Interfaces;
using ShipManagement.Controllers;
using ShipManagement.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ShipManagement.Tests.Controller
{
    public class ShipControllerTest
    {
        private readonly ShipController controller;
        private readonly Mock<IShipService> _mockService;
        private readonly Mock<IRedisCacheService> _mockRedisCacheService;

        public ShipControllerTest()
        {
            _mockService = new Mock<IShipService>();
            _mockRedisCacheService = new Mock<IRedisCacheService>();
            controller = new ShipController(_mockService.Object, _mockRedisCacheService.Object);
        }

        [Fact]
        public async Task GetShips_ShouldReturnOkResult()
        {
            // Arrange
            _mockService.Setup(s => s.GetShipsAsync()).ReturnsAsync(new List<ShipBasicDto>());

            // Act
            var result = await controller.GetShips();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetShipByCode_ShouldReturnOkResult_WhenShipExists()
        {
            // Arrange
            var shipCode = "AE-001";
            var shipDto = new ShipDetailDtoWithBasicUsers { ShipCode = shipCode, Id = 1, Name = "Test Ship" };
            _mockService.Setup(s => s.GetShipByCodeAsync(shipCode)).ReturnsAsync(shipDto);

            // Act
            var result = await controller.GetShipByCode(shipCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedShip = Assert.IsType<ShipDetailDtoWithBasicUsers>(okResult.Value);
            Assert.Equal(shipCode, returnedShip.ShipCode);
        }

        [Fact]
        public async Task GetShipByCode_ShouldReturnNotFound_WhenShipDoesNotExist()
        {
            // Arrange
            var shipCode = "AE-999";
            _mockService.Setup(s => s.GetShipByCodeAsync(shipCode)).ReturnsAsync((ShipDetailDtoWithBasicUsers?)null);

            // Act
            var result = await controller.GetShipByCode(shipCode);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetShips_ShouldReturnStatus500_OnException()
        {
            // Arrange
            _mockService.Setup(s => s.GetShipsAsync()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await controller.GetShips();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetShipByCode_ShouldReturnStatus500_OnException()
        {
            // Arrange
            var shipCode = "AE-001";
            _mockService.Setup(s => s.GetShipByCodeAsync(shipCode)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await controller.GetShipByCode(shipCode);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}