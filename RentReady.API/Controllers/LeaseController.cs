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
    public class LeasesController : ControllerBase
    {
        private readonly ILeaseService _leaseService;

        public LeasesController(ILeaseService leaseService)
        {
            _leaseService = leaseService;
        }

        // GET: api/leases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaseDto>>> GetAll(CancellationToken ct)
        {
            var leases = await _leaseService.GetAllAsync(ct);
            return Ok(leases);
        }

        // GET: api/leases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaseDto>> GetById(int id, CancellationToken ct)
        {
            var lease = await _leaseService.GetByIdAsync(id, ct);
            if (lease == null) return NotFound();
            return Ok(lease);
        }

        // POST: api/leases
        [HttpPost]
        public async Task<ActionResult<LeaseDto>> Create(LeaseForEditDto dto, CancellationToken ct)
        {
            var created = await _leaseService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/leases/5
        [HttpPut("{id}")]
        public async Task<ActionResult<LeaseDto>> Update(int id, LeaseForEditDto dto, CancellationToken ct)
        {
            try
            {
                var updated = await _leaseService.UpdateAsync(id, dto, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/leases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _leaseService.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}