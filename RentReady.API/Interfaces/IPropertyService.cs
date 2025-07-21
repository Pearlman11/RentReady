using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RentReady.API.Dtos;

namespace RentReady.API.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyDto>> GetAllAsync(CancellationToken ct = default);
        Task<PropertyDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PropertyDto> CreateAsync(PropertyForEditDto dto, CancellationToken ct = default);
        Task<PropertyDto> UpdateAsync(int id, PropertyForEditDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}