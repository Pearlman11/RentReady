using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RentReady.API.Dtos;

namespace RentReady.API.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync(CancellationToken ct = default);
        Task<PaymentDto?>            GetByIdAsync(int id, CancellationToken ct = default);
        Task<PaymentDto>             CreateAsync(PaymentForEditDto dto, CancellationToken ct = default);
        Task<PaymentDto>             UpdateAsync(int id, PaymentForEditDto dto, CancellationToken ct = default);
        Task                        DeleteAsync(int id, CancellationToken ct = default);
    }
}