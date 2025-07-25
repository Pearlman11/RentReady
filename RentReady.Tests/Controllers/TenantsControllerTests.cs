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
    public class TenantsControllerTests
    {
        private readonly Mock<ITenantService> _mockService;
        private readonly TenantsController    _controller;

        public TenantsControllerTests()
        {
            _mockService = new Mock<ITenantService>();
            _controller  = new TenantsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var list = new List<TenantDto> { new TenantDto { Id=1 } };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(list);

            var result = await _controller.GetAll(default);
            var ok     = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetById_NotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(9, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((TenantDto)null);

            var result = await _controller.GetById(9, default);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Found()
        {
            var dto = new TenantDto { Id=2 };
            _mockService.Setup(s => s.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(dto);

            var result = await _controller.GetById(2, default);
            var ok     = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var input  = new TenantForEditDto { Name="A", Email="a@x.com", PhoneNumber="111" };
            var output = new TenantDto { Id=3, Name="A", Email="a@x.com", PhoneNumber="111" };
            _mockService.Setup(s => s.CreateAsync(input, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(output);

            var result = await _controller.Create(input, default);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(output, created.Value);
        }

        [Fact]
        public async Task Update_NotFound()
        {
            _mockService.Setup(s => s.UpdateAsync(6, It.IsAny<TenantForEditDto>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Update(6, new TenantForEditDto(), default);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_Found()
        {
            var input  = new TenantForEditDto { Name="B", Email="b@x.com", PhoneNumber="222" };
            var output = new TenantDto { Id=6, Name="B", Email="b@x.com", PhoneNumber="222" };
            _mockService.Setup(s => s.UpdateAsync(6, input, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(output);

            var result = await _controller.Update(6, input, default);
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