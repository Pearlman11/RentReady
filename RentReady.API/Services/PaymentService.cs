using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentReady.API.Data;
using RentReady.API.Dtos;
using RentReady.API.Interfaces;
using RentReady.API.Models;

namespace RentReady.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly RentReadyContext _context;
        private readonly IMapper          _mapper;

        public PaymentService(RentReadyContext context, IMapper mapper)
        {
            _context = context;
            _mapper  = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _context.Payments
                                         .Include(p => p.Lease)
                                            .ThenInclude(l => l.Property)
                                         .Include(p => p.Lease)
                                            .ThenInclude(l => l.Tenant)
                                         .AsNoTracking()
                                         .ToListAsync(ct);
            return _mapper.Map<IEnumerable<PaymentDto>>(entities);
        }

        public async Task<PaymentDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Payments
                                       .Include(p => p.Lease)
                                          .ThenInclude(l => l.Property)
                                       .Include(p => p.Lease)
                                          .ThenInclude(l => l.Tenant)
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(p => p.Id == id, ct);
            if (entity == null) return null;
            return _mapper.Map<PaymentDto>(entity);
        }

        public async Task<PaymentDto> CreateAsync(PaymentForEditDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Payment>(dto);
            _context.Payments.Add(entity);
            await _context.SaveChangesAsync(ct);
            // Load navigation
            await _context.Entry(entity)
                          .Reference(e => e.Lease)
                          .Query()
                          .Include(l => l.Property)
                          .Include(l => l.Tenant)
                          .LoadAsync(ct);
            return _mapper.Map<PaymentDto>(entity);
        }

        public async Task<PaymentDto> UpdateAsync(int id, PaymentForEditDto dto, CancellationToken ct = default)
        {
            var entity = await _context.Payments.FindAsync(new object[]{ id }, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found.");

            _mapper.Map(dto, entity);
            _context.Payments.Update(entity);
            await _context.SaveChangesAsync(ct);

            // Reload navigation
            await _context.Entry(entity)
                          .Reference(e => e.Lease)
                          .Query()
                          .Include(l => l.Property)
                          .Include(l => l.Tenant)
                          .LoadAsync(ct);
            return _mapper.Map<PaymentDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Payments.FindAsync(new object[]{ id }, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found.");

            _context.Payments.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}