
namespace ShipManagement.Tests.Controller
{
    public class ShipControllerTest
    {
        [Theory, AutoMoqData]
        public async Task GetShipByCodeAsync_ReturnsOk_WhenShipExists(
            string shipCode,
            ShipDetails shipResponse,
            [Frozen] Mock<IShipService> mockService)
        {
            mockService.Setup(s => s.GetShipByCodeAsync(shipCode)).ReturnsAsync(shipResponse);
            var controller = new ShipController(mockService.Object);

            var result = await controller.GetShipByCodeAsync(shipCode);
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeOfType<ShipDetails>();
            ((ShipDetails)okResult.Value).Id.Should().Be(shipResponse.Id);
        }

        [Theory, AutoMoqData]
        public async Task GetShipsAsync_ReturnsOk(
            List<ShipResponse> response,
            [Frozen] Mock<IShipService> mockService)
        {
            mockService.Setup(s => s.GetShipsAsync()).ReturnsAsync(response);
            var controller = new ShipController(mockService.Object);

            var result = await controller.GetShipsAsync();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeOfType<List<ShipResponse>>();
            ((List<ShipResponse>)okResult.Value).Count.Should().Be(response.Count);
        }

        [Theory, AutoMoqData]
        public async Task GetShipByCodeAsync_ReturnsNotFound_WhenShipDoesNotExist(
            string shipCode,
            [Frozen] Mock<IShipService> mockService)
        {
            mockService.Setup(s => s.GetShipByCodeAsync(shipCode)).ReturnsAsync((ShipDetails?)null);
            var controller = new ShipController(mockService.Object);

            var result = await controller.GetShipByCodeAsync(shipCode);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoMoqData]
        public async Task CreateShipAsync_ReturnsOk(
            CreateShipRequest newShip,
            ShipResponse createdShip,
            [Frozen] Mock<IShipService> mockService)
        {
            mockService.Setup(s => s.CreateShipAsync(newShip)).ReturnsAsync(createdShip);
            var controller = new ShipController(mockService.Object);
            var result = await controller.CreateShipAsync(newShip);
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeOfType<ShipResponse>();
            ((ShipResponse)okResult.Value).Id.Should().Be(createdShip.Id);
        }

        [Theory, AutoMoqData]
        public async Task UpdateShipVelocityAsync_ReturnsOk(
            string shipCode,
            UpdateShipVelocityRequest request,
            [Frozen] Mock<IShipService> mockService)
        {
            mockService.Setup(s => s.UpdateShipVelocityAsync(shipCode, request)).Returns(Task.CompletedTask);
            var controller = new ShipController(mockService.Object);
            var result = await controller.UpdateShipVelocityAsync(shipCode, request);
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
        }
        [Theory, AutoMoqData]
        public async Task GetClosestPortAsync_ReturnsOk(
            string shipCode,
            ShipClosestPortResponse closestPort,
            [Frozen] Mock<IShipService> mockService)
        {
            mockService.Setup(s => s.GetClosestPortAsync(shipCode)).ReturnsAsync(closestPort);
            var controller = new ShipController(mockService.Object);
            var result = await controller.GetClosestPortAsync(shipCode);
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeOfType<ShipClosestPortResponse>();
            ((ShipClosestPortResponse)okResult.Value).PortName.Should().Be(closestPort.PortName);
        }
    }
}
