using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RentReady.API.Dtos;

namespace RentReady.API.Interfaces
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantDto>> GetAllAsync(CancellationToken ct = default);
        Task<TenantDto?>            GetByIdAsync(int id, CancellationToken ct = default);
        Task<TenantDto>             CreateAsync(TenantForEditDto dto, CancellationToken ct = default);
        Task<TenantDto>             UpdateAsync(int id, TenantForEditDto dto, CancellationToken ct = default);
        Task                        DeleteAsync(int id, CancellationToken ct = default);
    }
}