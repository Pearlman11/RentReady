using System.Collections.Generic;
using System.Linq;
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
    public class LeaseService : ILeaseService
    {
        private readonly RentReadyContext _context;
        private readonly IMapper _mapper;

        public LeaseService(RentReadyContext context, IMapper mapper)
        {
            _context = context;
            _mapper  = mapper;
        }

        public async Task<IEnumerable<LeaseDto>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _context.Leases
                                         .Include(l => l.Property)
                                         .Include(l => l.Tenant)
                                         .AsNoTracking()
                                         .ToListAsync(ct);
            return _mapper.Map<IEnumerable<LeaseDto>>(entities);
        }

        public async Task<LeaseDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Leases
                                       .Include(l => l.Property)
                                       .Include(l => l.Tenant)
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(l => l.Id == id, ct);
            if (entity == null) return null;
            return _mapper.Map<LeaseDto>(entity);
        }

        public async Task<LeaseDto> CreateAsync(LeaseForEditDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Lease>(dto);
            _context.Leases.Add(entity);
            await _context.SaveChangesAsync(ct);
            // Reload with navigation props
            await _context.Entry(entity).Reference(e => e.Property).LoadAsync(ct);
            await _context.Entry(entity).Reference(e => e.Tenant).LoadAsync(ct);
            return _mapper.Map<LeaseDto>(entity);
        }

        public async Task<LeaseDto> UpdateAsync(int id, LeaseForEditDto dto, CancellationToken ct = default)
        {
            var entity = await _context.Leases.FindAsync(new object[]{ id }, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Lease with ID {id} not found.");

            _mapper.Map(dto, entity);
            _context.Leases.Update(entity);
            await _context.SaveChangesAsync(ct);

            // Reload expanded props
            await _context.Entry(entity).Reference(e => e.Property).LoadAsync(ct);
            await _context.Entry(entity).Reference(e => e.Tenant).LoadAsync(ct);
            return _mapper.Map<LeaseDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Leases.FindAsync(new object[]{ id }, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Lease with ID {id} not found.");

            _context.Leases.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}