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
    public class PaymentServiceTests
    {
        private readonly IMapper _mapper;

        public PaymentServiceTests()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            _mapper = cfg.CreateMapper();
        }

        private RentReadyContext GetContext(string dbName)
        {
            var opts = new DbContextOptionsBuilder<RentReadyContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new RentReadyContext(opts);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsPaymentsWithIncludes()
        {
            // Arrange
            var ctx = GetContext(nameof(GetAllAsync_ReturnsPaymentsWithIncludes));
            var prop   = new Property { Address="A", Unit="1", RentAmount=100m };
            var tenant = new Tenant   { Name="T", Email="t@x.com", PhoneNumber="111" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease = new Lease { PropertyId=prop.Id, TenantId=tenant.Id, StartDate=DateTime.Today };
            ctx.Leases.Add(lease);
            await ctx.SaveChangesAsync();

            var p1 = new Payment { LeaseId=lease.Id, Amount=100m, Date=DateTime.Today };
            var p2 = new Payment { LeaseId=lease.Id, Amount=200m, Date=DateTime.Today.AddDays(1) };
            ctx.Payments.AddRange(p1, p2);
            await ctx.SaveChangesAsync();

            var svc = new PaymentService(ctx, _mapper);

            // Act
            var results = await svc.GetAllAsync();

            // Assert
            Assert.Equal(2, results.Count());
            Assert.All(results, x =>
            {
                Assert.NotNull(x.Lease);
                Assert.NotNull(x.Lease.Property);
                Assert.NotNull(x.Lease.Tenant);
            });
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectOrNull()
        {
            var ctx = GetContext(nameof(GetByIdAsync_ReturnsCorrectOrNull));
            var prop   = new Property { Address="A", Unit="1", RentAmount=100m };
            var tenant = new Tenant   { Name="T", Email="t@x.com", PhoneNumber="111" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease = new Lease { PropertyId=prop.Id, TenantId=tenant.Id, StartDate=DateTime.Today };
            ctx.Leases.Add(lease);
            await ctx.SaveChangesAsync();

            var payment = new Payment { LeaseId=lease.Id, Amount=123m, Date=DateTime.Today };
            ctx.Payments.Add(payment);
            await ctx.SaveChangesAsync();

            var svc = new PaymentService(ctx, _mapper);

            var found = await svc.GetByIdAsync(payment.Id);
            var missing = await svc.GetByIdAsync(-1);

            Assert.NotNull(found);
            Assert.Equal(payment.Id, found.Id);
            Assert.Null(missing);
        }

        [Fact]
        public async Task CreateAsync_CreatesAndReturnsDto()
        {
            var ctx = GetContext(nameof(CreateAsync_CreatesAndReturnsDto));
            var prop   = new Property { Address="A", Unit="1", RentAmount=100m };
            var tenant = new Tenant   { Name="T", Email="t@x.com", PhoneNumber="111" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease = new Lease { PropertyId=prop.Id, TenantId=tenant.Id, StartDate=DateTime.Today };
            ctx.Leases.Add(lease);
            await ctx.SaveChangesAsync();

            var svc = new PaymentService(ctx, _mapper);
            var edit = new RentReady.API.Dtos.PaymentForEditDto
            {
                LeaseId = lease.Id,
                Amount  = 555m,
                Date    = DateTime.Today
            };

            var dto = await svc.CreateAsync(edit);

            Assert.True(dto.Id > 0);
            Assert.Equal(lease.Id, dto.LeaseId);
            Assert.Equal(555m, dto.Amount);
            Assert.NotNull(dto.Lease);
        }

        [Fact]
        public async Task UpdateAsync_Existing_UpdatesFields()
        {
            var ctx = GetContext(nameof(UpdateAsync_Existing_UpdatesFields));
            var prop   = new Property { Address="A", Unit="1", RentAmount=100m };
            var tenant = new Tenant   { Name="T", Email="t@x.com", PhoneNumber="111" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease = new Lease { PropertyId=prop.Id, TenantId=tenant.Id, StartDate=DateTime.Today };
            ctx.Leases.Add(lease);
            await ctx.SaveChangesAsync();

            var payment = new Payment { LeaseId=lease.Id, Amount=100m, Date=DateTime.Today };
            ctx.Payments.Add(payment);
            await ctx.SaveChangesAsync();

            var svc = new PaymentService(ctx, _mapper);
            var edit = new RentReady.API.Dtos.PaymentForEditDto
            {
                LeaseId = lease.Id,
                Amount  = 777m,
                Date    = DateTime.Today.AddDays(2)
            };

            var updated = await svc.UpdateAsync(payment.Id, edit);

            Assert.Equal(payment.Id, updated.Id);
            Assert.Equal(777m, updated.Amount);
            Assert.Equal(edit.Date, updated.Date);
        }

        [Fact]
        public async Task UpdateAsync_NonExistent_Throws()
        {
            var svc = new PaymentService(GetContext(nameof(UpdateAsync_NonExistent_Throws)), _mapper);
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                svc.UpdateAsync(999, new RentReady.API.Dtos.PaymentForEditDto { LeaseId=1, Amount=1m, Date=DateTime.Today })
            );
        }

        [Fact]
        public async Task DeleteAsync_Existing_Removes()
        {
            var ctx = GetContext(nameof(DeleteAsync_Existing_Removes));
            var prop   = new Property { Address="A", Unit="1", RentAmount=100m };
            var tenant = new Tenant   { Name="T", Email="t@x.com", PhoneNumber="111" };
            ctx.Properties.Add(prop);
            ctx.Tenants.Add(tenant);
            await ctx.SaveChangesAsync();

            var lease = new Lease { PropertyId=prop.Id, TenantId=tenant.Id, StartDate=DateTime.Today };
            ctx.Leases.Add(lease);
            await ctx.SaveChangesAsync();

            var payment = new Payment { LeaseId=lease.Id, Amount=100m, Date=DateTime.Today };
            ctx.Payments.Add(payment);
            await ctx.SaveChangesAsync();

            var svc = new PaymentService(ctx, _mapper);
            await svc.DeleteAsync(payment.Id);

            var check = await ctx.Payments.FindAsync(payment.Id);
            Assert.Null(check);
        }

        [Fact]
        public async Task DeleteAsync_NonExistent_Throws()
        {
            var svc = new PaymentService(GetContext(nameof(DeleteAsync_NonExistent_Throws)), _mapper);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.DeleteAsync(-1));
        }
    }
}