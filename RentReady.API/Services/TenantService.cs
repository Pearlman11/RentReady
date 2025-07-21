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
    public class TenantService : ITenantService
    {
        private readonly RentReadyContext _context;
        private readonly IMapper         _mapper;

        public TenantService(RentReadyContext context, IMapper mapper)
        {
            _context = context;
            _mapper  = mapper;
        }

        public async Task<IEnumerable<TenantDto>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _context.Tenants
                                         .Include(t => t.Leases)
                                         .AsNoTracking()
                                         .ToListAsync(ct);
            return _mapper.Map<IEnumerable<TenantDto>>(entities);
        }

        public async Task<TenantDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Tenants
                                       .Include(t => t.Leases)
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(t => t.Id == id, ct);
            if (entity == null) return null;
            return _mapper.Map<TenantDto>(entity);
        }

        public async Task<TenantDto> CreateAsync(TenantForEditDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Tenant>(dto);
            _context.Tenants.Add(entity);
            await _context.SaveChangesAsync(ct);
            // No leases yet, so mapping immediately is fine
            return _mapper.Map<TenantDto>(entity);
        }

        public async Task<TenantDto> UpdateAsync(int id, TenantForEditDto dto, CancellationToken ct = default)
        {
            var entity = await _context.Tenants.FindAsync(new object[]{ id }, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Tenant with ID {id} not found.");

            _mapper.Map(dto, entity);
            _context.Tenants.Update(entity);
            await _context.SaveChangesAsync(ct);

            // Reload leases
            await _context.Entry(entity).Collection(e => e.Leases).LoadAsync(ct);
            return _mapper.Map<TenantDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Tenants.FindAsync(new object[]{ id }, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Tenant with ID {id} not found.");

            _context.Tenants.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}