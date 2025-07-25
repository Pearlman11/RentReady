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
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        // GET: api/tenants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetAll(CancellationToken ct)
        {
            var tenants = await _tenantService.GetAllAsync(ct);
            return Ok(tenants);
        }

        // GET: api/tenants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TenantDto>> GetById(int id, CancellationToken ct)
        {
            var tenant = await _tenantService.GetByIdAsync(id, ct);
            if (tenant == null) return NotFound();
            return Ok(tenant);
        }

        // POST: api/tenants
        [HttpPost]
        public async Task<ActionResult<TenantDto>> Create(TenantForEditDto dto, CancellationToken ct)
        {
            var created = await _tenantService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/tenants/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TenantDto>> Update(int id, TenantForEditDto dto, CancellationToken ct)
        {
            try
            {
                var updated = await _tenantService.UpdateAsync(id, dto, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/tenants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _tenantService.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}