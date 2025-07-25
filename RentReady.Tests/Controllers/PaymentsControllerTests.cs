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
    public class PaymentsControllerTests
    {
        private readonly Mock<IPaymentService> _mockService;
        private readonly PaymentsController     _controller;

        public PaymentsControllerTests()
        {
            _mockService = new Mock<IPaymentService>();
            _controller  = new PaymentsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var list = new List<PaymentDto> { new PaymentDto { Id=1 } };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(list);

            var result = await _controller.GetAll(default);
            var ok     = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetById_NotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((PaymentDto)null);

            var result = await _controller.GetById(10, default);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Found()
        {
            var dto = new PaymentDto { Id=2 };
            _mockService.Setup(s => s.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(dto);

            var result = await _controller.GetById(2, default);
            var ok     = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var input  = new PaymentForEditDto { LeaseId=1, Amount=100m, Date=System.DateTime.Today };
            var output = new PaymentDto { Id=3, LeaseId=1, Amount=100m, Date=System.DateTime.Today };
            _mockService.Setup(s => s.CreateAsync(input, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(output);

            var result  = await _controller.Create(input, default);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(output, created.Value);
        }

        [Fact]
        public async Task Update_NotFound()
        {
            _mockService.Setup(s => s.UpdateAsync(5, It.IsAny<PaymentForEditDto>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Update(5, new PaymentForEditDto(), default);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_Found()
        {
            var input  = new PaymentForEditDto { LeaseId=1, Amount=200m, Date=System.DateTime.Today };
            var output = new PaymentDto { Id=5, LeaseId=1, Amount=200m, Date=System.DateTime.Today };
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