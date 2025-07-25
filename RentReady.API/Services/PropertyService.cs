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
    public class PropertyService : IPropertyService
    {
        private readonly RentReadyContext _context;
        private readonly IMapper _mapper;

        public PropertyService(RentReadyContext context, IMapper mapper)
        {
            _context = context;
            _mapper  = mapper;
        }

        public async Task<IEnumerable<PropertyDto>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _context.Properties
                                         .AsNoTracking()
                                         .ToListAsync(ct);
            return _mapper.Map<IEnumerable<PropertyDto>>(entities);
        }

        public async Task<PropertyDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Properties
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(p => p.Id == id, ct);
            if (entity == null) return null;
            return _mapper.Map<PropertyDto>(entity);
        }

        public async Task<PropertyDto> CreateAsync(PropertyForEditDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Property>(dto);
            _context.Properties.Add(entity);
            await _context.SaveChangesAsync(ct);
            return _mapper.Map<PropertyDto>(entity);
        }

        public async Task<PropertyDto> UpdateAsync(int id, PropertyForEditDto dto, CancellationToken ct = default)
        {
            var entity = await _context.Properties.FindAsync(new object[]{ id }, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Property with ID {id} not found.");

            // Map incoming fields onto the tracked entity
            _mapper.Map(dto, entity);

            _context.Properties.Update(entity);
            await _context.SaveChangesAsync(ct);

            return _mapper.Map<PropertyDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Properties.FindAsync(new object[]{ id }, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Property with ID {id} not found.");

            _context.Properties.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}