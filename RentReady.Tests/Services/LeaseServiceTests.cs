using System;
using System.Collections.Generic;
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
    public class LeaseServiceTests
    {
        private readonly IMapper _mapper;

        public LeaseServiceTests()
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
        public async Task GetAllAsync_ReturnsAllLeasesWithIncludes()
        {
            // Arrange
            var ctx = GetContext(nameof(GetAllAsync_ReturnsAllLeasesWithIncludes));
            var prop = new Property { Address="X", Unit="1", RentAmount=100 };
            var tenant = new Tenant { Name="T", Email="t@x.com", PhoneNumber="000" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease1 = new Lease { PropertyId = prop.Id, TenantId = tenant.Id, StartDate = DateTime.Today };
            var lease2 = new Lease { PropertyId = prop.Id, TenantId = tenant.Id, StartDate = DateTime.Today.AddDays(1) };
            ctx.Leases.AddRange(lease1, lease2);
            await ctx.SaveChangesAsync();

            var svc = new LeaseService(ctx, _mapper);

            // Act
            var results = await svc.GetAllAsync();

            // Assert
            Assert.Equal(2, results.Count());
            Assert.All(results, l =>
            {
                Assert.NotNull(l.Property);
                Assert.NotNull(l.Tenant);
            });
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectLeaseOrNull()
        {
            var ctx = GetContext(nameof(GetByIdAsync_ReturnsCorrectLeaseOrNull));
            var prop = new Property { Address="X", Unit="1", RentAmount=100 };
            var tenant = new Tenant { Name="T", Email="t@x.com", PhoneNumber="000" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease = new Lease { PropertyId = prop.Id, TenantId = tenant.Id, StartDate = DateTime.Today };
            ctx.Leases.Add(lease);
            await ctx.SaveChangesAsync();
            var svc = new LeaseService(ctx, _mapper);

            var found = await svc.GetByIdAsync(lease.Id);
            var missing = await svc.GetByIdAsync(-1);

            Assert.NotNull(found);
            Assert.Equal(lease.Id, found.Id);
            Assert.Null(missing);
        }

        [Fact]
        public async Task CreateAsync_SavesAndReturnsDtoWithIncludes()
        {
            var ctx = GetContext(nameof(CreateAsync_SavesAndReturnsDtoWithIncludes));
            var prop = new Property { Address="X", Unit="1", RentAmount=100 };
            var tenant = new Tenant { Name="T", Email="t@x.com", PhoneNumber="000" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();
            var svc = new LeaseService(ctx, _mapper);

            var edit = new RentReady.API.Dtos.LeaseForEditDto {
                PropertyId = prop.Id,
                TenantId   = tenant.Id,
                StartDate  = DateTime.Today,
                EndDate    = null
            };

            var dto = await svc.CreateAsync(edit);

            Assert.True(dto.Id > 0);
            Assert.Equal(prop.Id, dto.PropertyId);
            Assert.Equal(tenant.Id, dto.TenantId);
            Assert.NotNull(dto.Property);
            Assert.NotNull(dto.Tenant);
        }

        [Fact]
        public async Task UpdateAsync_Existing_UpdatesFields()
        {
            var ctx = GetContext(nameof(UpdateAsync_Existing_UpdatesFields));
            var prop = new Property { Address="X", Unit="1", RentAmount=100 };
            var tenant = new Tenant { Name="T", Email="t@x.com", PhoneNumber="000" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease = new Lease { PropertyId = prop.Id, TenantId = tenant.Id, StartDate = DateTime.Today };
            ctx.Leases.Add(lease);
            await ctx.SaveChangesAsync();

            var svc = new LeaseService(ctx, _mapper);
            var edit = new RentReady.API.Dtos.LeaseForEditDto {
                PropertyId = prop.Id,
                TenantId   = tenant.Id,
                StartDate  = DateTime.Today.AddDays(5),
                EndDate    = DateTime.Today.AddDays(10)
            };

            var updated = await svc.UpdateAsync(lease.Id, edit);

            Assert.Equal(lease.Id, updated.Id);
            Assert.Equal(edit.StartDate, updated.StartDate);
            Assert.Equal(edit.EndDate, updated.EndDate);
        }

        [Fact]
        public async Task UpdateAsync_NonExistent_Throws()
        {
            var svc = new LeaseService(GetContext(nameof(UpdateAsync_NonExistent_Throws)), _mapper);
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                svc.UpdateAsync(999, new RentReady.API.Dtos.LeaseForEditDto {
                    PropertyId = 1, TenantId = 1, StartDate = DateTime.Today
                })
            );
        }

        [Fact]
        public async Task DeleteAsync_Existing_Removes()
        {
            var ctx = GetContext(nameof(DeleteAsync_Existing_Removes));
            var prop = new Property { Address="X", Unit="1", RentAmount=100 };
            var tenant = new Tenant { Name="T", Email="t@x.com", PhoneNumber="000" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease = new Lease { PropertyId = prop.Id, TenantId = tenant.Id, StartDate = DateTime.Today };
            ctx.Leases.Add(lease);
            await ctx.SaveChangesAsync();

            var svc = new LeaseService(ctx, _mapper);
            await svc.DeleteAsync(lease.Id);

            var check = await ctx.Leases.FindAsync(lease.Id);
            Assert.Null(check);
        }

        [Fact]
        public async Task DeleteAsync_NonExistent_Throws()
        {
            var svc = new LeaseService(GetContext(nameof(DeleteAsync_NonExistent_Throws)), _mapper);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.DeleteAsync(-1));
        }
    }
}