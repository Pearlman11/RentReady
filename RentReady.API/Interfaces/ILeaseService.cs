using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RentReady.API.Dtos;

namespace RentReady.API.Interfaces
{
    public interface ILeaseService
    {
        Task<IEnumerable<LeaseDto>> GetAllAsync(CancellationToken ct = default);
        Task<LeaseDto?>            GetByIdAsync(int id, CancellationToken ct = default);
        Task<LeaseDto>             CreateAsync(LeaseForEditDto dto, CancellationToken ct = default);
        Task<LeaseDto>             UpdateAsync(int id, LeaseForEditDto dto, CancellationToken ct = default);
        Task                       DeleteAsync(int id, CancellationToken ct = default);
    }
}