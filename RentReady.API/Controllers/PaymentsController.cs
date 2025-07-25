using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RentReady.API.Dtos;
using RentReady.API.Interfaces;

namespace RentReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: api/payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll(CancellationToken ct)
        {
            var payments = await _paymentService.GetAllAsync(ct);
            return Ok(payments);
        }

        // GET: api/payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetById(int id, CancellationToken ct)
        {
            var payment = await _paymentService.GetByIdAsync(id, ct);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        // POST: api/payments
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create(PaymentForEditDto dto, CancellationToken ct)
        {
            var created = await _paymentService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/payments/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentDto>> Update(int id, PaymentForEditDto dto, CancellationToken ct)
        {
            try
            {
                var updated = await _paymentService.UpdateAsync(id, dto, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/payments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _paymentService.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}