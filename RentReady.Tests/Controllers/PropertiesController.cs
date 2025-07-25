using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RentReady.API.Controllers;
using RentReady.API.Dtos;
using RentReady.API.Interfaces;
using Xunit;

namespace RentReady.Tests.Controllers
{
    public class PropertiesControllerTests
    {
        private readonly Mock<IPropertyService> _mockService;
        private readonly PropertiesController   _controller;

        public PropertiesControllerTests()
        {
            _mockService = new Mock<IPropertyService>();
            _controller  = new PropertiesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            // Arrange
            var props = new List<PropertyDto>
            {
                new PropertyDto { Id=1, Address="A", Unit="U", RentAmount=100m }
            };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(props);

            // Act
            var result = await _controller.GetAll(default);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(props, ok.Value);
        }

        [Fact]
        public async Task GetById_NotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((PropertyDto)null);

            var result = await _controller.GetById(5, default);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Found()
        {
            var dto = new PropertyDto { Id=2, Address="B", Unit="V", RentAmount=200m };
            _mockService.Setup(s => s.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(dto);

            var result = await _controller.GetById(2, default);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var input = new PropertyForEditDto { Address="C", Unit="W", RentAmount=300m };
            var output = new PropertyDto { Id=3, Address="C", Unit="W", RentAmount=300m };
            _mockService.Setup(s => s.CreateAsync(input, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(output);

            var result = await _controller.Create(input, default);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PropertiesController.GetById), created.ActionName);
            Assert.Equal(output, created.Value);
        }

        [Fact]
        public async Task Update_NotFound()
        {
            _mockService.Setup(s => s.UpdateAsync(4, It.IsAny<PropertyForEditDto>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Update(4, new PropertyForEditDto(), default);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_Found()
        {
            var input = new PropertyForEditDto { Address="D", Unit="X", RentAmount=400m };
            var output = new PropertyDto { Id=4, Address="D", Unit="X", RentAmount=400m };
            _mockService.Setup(s => s.UpdateAsync(4, input, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(output);

            var result = await _controller.Update(4, input, default);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(output, ok.Value);
        }

        [Fact]
        public async Task Delete_NotFound()
        {
            _mockService.Setup(s => s.DeleteAsync(5, It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Delete(5, default);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_NoContent()
        {
            _mockService.Setup(s => s.DeleteAsync(6, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var result = await _controller.Delete(6, default);
            Assert.IsType<NoContentResult>(result);
        }
    }
}