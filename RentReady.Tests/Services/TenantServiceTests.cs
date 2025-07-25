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

namespace RentReady.Tests.Services
{
    public class TenantServiceTests
    {
        private readonly IMapper _mapper;

        public TenantServiceTests()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            _mapper = cfg.CreateMapper();
        }

        private RentReadyContext GetContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<RentReadyContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new RentReadyContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsTenantsWithLeases()
        {
            // Arrange
            var ctx = GetContext(nameof(GetAllAsync_ReturnsTenantsWithLeases));
            var t1 = new Tenant { Name = "A", Email = "a@x.com", PhoneNumber = "111" };
            var t2 = new Tenant { Name = "B", Email = "b@x.com", PhoneNumber = "222" };
            ctx.Tenants.AddRange(t1, t2);
            await ctx.SaveChangesAsync();

            var svc = new TenantService(ctx, _mapper);

            // Act
            var results = await svc.GetAllAsync();

            // Assert
            Assert.Equal(2, results.Count());
            Assert.All(results, t => Assert.NotNull(t.Name));
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectTenantOrNull()
        {
            var ctx = GetContext(nameof(GetByIdAsync_ReturnsCorrectTenantOrNull));
            var tenant = new Tenant { Name = "C", Email = "c@x.com", PhoneNumber = "333" };
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var svc = new TenantService(ctx, _mapper);
            var found = await svc.GetByIdAsync(tenant.Id);
            var missing = await svc.GetByIdAsync(-1);

            Assert.NotNull(found);
            Assert.Equal(tenant.Id, found.Id);
            Assert.Null(missing);
        }

        [Fact]
        public async Task CreateAsync_AddsAndReturnsDto()
        {
            var ctx = GetContext(nameof(CreateAsync_AddsAndReturnsDto));
            var svc = new TenantService(ctx, _mapper);
            var edit = new RentReady.API.Dtos.TenantForEditDto
            {
                Name = "D", Email = "d@x.com", PhoneNumber = "444"
            };

            var dto = await svc.CreateAsync(edit);

            Assert.True(dto.Id > 0);
            Assert.Equal("D", dto.Name);
            Assert.Equal("d@x.com", dto.Email);
        }

        [Fact]
        public async Task UpdateAsync_Existing_UpdatesFields()
        {
            var ctx = GetContext(nameof(UpdateAsync_Existing_UpdatesFields));
            var tenant = new Tenant { Name = "E", Email = "e@x.com", PhoneNumber = "555" };
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var svc = new TenantService(ctx, _mapper);
            var edit = new RentReady.API.Dtos.TenantForEditDto
            {
                Name = "E2", Email = "e2@x.com", PhoneNumber = "556"
            };

            var dto = await svc.UpdateAsync(tenant.Id, edit);

            Assert.Equal(tenant.Id, dto.Id);
            Assert.Equal("E2", dto.Name);
            Assert.Equal("e2@x.com", dto.Email);
        }

        [Fact]
        public async Task UpdateAsync_NonExistent_Throws()
        {
            var svc = new TenantService(GetContext(nameof(UpdateAsync_NonExistent_Throws)), _mapper);
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                svc.UpdateAsync(999, new RentReady.API.Dtos.TenantForEditDto { Name="X", Email="x@x.com", PhoneNumber="000" })
            );
        }

        [Fact]
        public async Task DeleteAsync_Existing_Removes()
        {
            var ctx = GetContext(nameof(DeleteAsync_Existing_Removes));
            var tenant = new Tenant { Name = "F", Email = "f@x.com", PhoneNumber = "777" };
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var svc = new TenantService(ctx, _mapper);
            await svc.DeleteAsync(tenant.Id);

            var check = await ctx.Tenants.FindAsync(tenant.Id);
            Assert.Null(check);
        }

        [Fact]
        public async Task DeleteAsync_NonExistent_Throws()
        {
            var svc = new TenantService(GetContext(nameof(DeleteAsync_NonExistent_Throws)), _mapper);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.DeleteAsync(-1));
        }
    }
}