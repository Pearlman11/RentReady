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
    public class LeasesControllerTests
    {
        private readonly Mock<ILeaseService> _mockService;
        private readonly LeasesController    _controller;

        public LeasesControllerTests()
        {
            _mockService = new Mock<ILeaseService>();
            _controller  = new LeasesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var items = new List<LeaseDto> { new LeaseDto { Id=1 } };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(items);

            var result = await _controller.GetAll(default);
            var ok     = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetById_NotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((LeaseDto)null);

            var result = await _controller.GetById(10, default);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Found()
        {
            var dto = new LeaseDto { Id=2 };
            _mockService.Setup(s => s.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(dto);

            var result = await _controller.GetById(2, default);
            var ok     = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var input = new LeaseForEditDto { PropertyId=1, TenantId=1, StartDate=System.DateTime.Today };
            var output = new LeaseDto { Id=3, PropertyId=1, TenantId=1 };
            _mockService.Setup(s => s.CreateAsync(input, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(output);

            var result  = await _controller.Create(input, default);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(output, created.Value);
        }

        [Fact]
        public async Task Update_NotFound()
        {
            _mockService.Setup(s => s.UpdateAsync(5, It.IsAny<LeaseForEditDto>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Update(5, new LeaseForEditDto(), default);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_Found()
        {
            var input = new LeaseForEditDto { PropertyId=1, TenantId=1, StartDate=System.DateTime.Today };
            var output = new LeaseDto { Id=5 };
            _mockService.Setup(s => s.UpdateAsync(5, input, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(output);

            var result = await _controller.Update(5, input, default);
            var ok     = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(output, ok.Value);
        }

        [Fact]
        public async Task Delete_NotFound()
        {
            _mockService.Setup(s => s.DeleteAsync(7, It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Delete(7, default);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_NoContent()
        {
            _mockService.Setup(s => s.DeleteAsync(8, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var result = await _controller.Delete(8, default);
            Assert.IsType<NoContentResult>(result);
        }
    }
}