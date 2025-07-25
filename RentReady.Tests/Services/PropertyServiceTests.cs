using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentReady.API.Data;
using RentReady.API.Mapping;
using RentReady.API.Models;
using RentReady.API.Services;
using Xunit;

namespace RentReady.Tests
{
    public class PropertyServiceTests
    {
        private readonly IMapper _mapper;

        public PropertyServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        private RentReadyContext GetContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<RentReadyContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new RentReadyContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProperties()
        {
            // Arrange
            var context = GetContext(nameof(GetAllAsync_ReturnsAllProperties));
            context.Properties.AddRange(
                new Property { Address = "Addr1", Unit = "U1", RentAmount = 100m },
                new Property { Address = "Addr2", Unit = "U2", RentAmount = 200m }
            );
            await context.SaveChangesAsync();
            var service = new PropertyService(context, _mapper);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectProperty()
        {
            var context = GetContext(nameof(GetByIdAsync_ReturnsCorrectProperty));
            var p = new Property { Address = "A", Unit = "U", RentAmount = 150m };
            context.Properties.Add(p);
            await context.SaveChangesAsync();
            var service = new PropertyService(context, _mapper);

            var dto = await service.GetByIdAsync(p.Id);

            Assert.NotNull(dto);
            Assert.Equal(p.Id, dto.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNullWhenNotFound()
        {
            var service = new PropertyService(GetContext(nameof(GetByIdAsync_ReturnsNullWhenNotFound)), _mapper);
            var dto = await service.GetByIdAsync(-1);
            Assert.Null(dto);
        }

        [Fact]
        public async Task CreateAsync_AddsAndReturnsDto()
        {
            var context = GetContext(nameof(CreateAsync_AddsAndReturnsDto));
            var service = new PropertyService(context, _mapper);
            var edit = new RentReady.API.Dtos.PropertyForEditDto
            {
                Address = "New", Unit = "N", RentAmount = 300m
            };

            var dto = await service.CreateAsync(edit);

            Assert.True(dto.Id > 0);
            Assert.Equal("New", dto.Address);
            Assert.Equal("N", dto.Unit);
            Assert.Equal(300m, dto.RentAmount);
        }

        [Fact]
        public async Task UpdateAsync_Existing_UpdatesFields()
        {
            var context = GetContext(nameof(UpdateAsync_Existing_UpdatesFields));
            var p = new Property { Address = "Old", Unit = "O", RentAmount = 100m };
            context.Properties.Add(p);
            await context.SaveChangesAsync();
            var service = new PropertyService(context, _mapper);
            var edit = new RentReady.API.Dtos.PropertyForEditDto
            {
                Address = "Upd", Unit = "U", RentAmount = 400m
            };

            var dto = await service.UpdateAsync(p.Id, edit);

            Assert.Equal(p.Id, dto.Id);
            Assert.Equal("Upd", dto.Address);
            Assert.Equal("U", dto.Unit);
            Assert.Equal(400m, dto.RentAmount);
        }

        [Fact]
        public async Task UpdateAsync_NonExistent_Throws()
        {
            var service = new PropertyService(GetContext(nameof(UpdateAsync_NonExistent_Throws)), _mapper);
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.UpdateAsync(999, new RentReady.API.Dtos.PropertyForEditDto { Address="x", Unit="u", RentAmount=0m })
            );
        }

        [Fact]
        public async Task DeleteAsync_Existing_Removes()
        {
            var context = GetContext(nameof(DeleteAsync_Existing_Removes));
            var p = new Property { Address = "ToDel", Unit = "T", RentAmount = 50m };
            context.Properties.Add(p);
            await context.SaveChangesAsync();
            var service = new PropertyService(context, _mapper);

            await service.DeleteAsync(p.Id);
            var entity = await context.Properties.FindAsync(p.Id);
            Assert.Null(entity);
        }

        [Fact]
        public async Task DeleteAsync_NonExistent_Throws()
        {
            var service = new PropertyService(GetContext(nameof(DeleteAsync_NonExistent_Throws)), _mapper);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(-1));
        }
    }
}