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
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // GET: api/properties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAll(CancellationToken ct)
        {
            var props = await _propertyService.GetAllAsync(ct);
            return Ok(props);
        }

        // GET: api/properties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetById(int id, CancellationToken ct)
        {
            var prop = await _propertyService.GetByIdAsync(id, ct);
            if (prop == null) return NotFound();
            return Ok(prop);
        }

        // POST: api/properties
        [HttpPost]
        public async Task<ActionResult<PropertyDto>> Create(PropertyForEditDto dto, CancellationToken ct)
        {
            var created = await _propertyService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/properties/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PropertyDto>> Update(int id, PropertyForEditDto dto, CancellationToken ct)
        {
            try
            {
                var updated = await _propertyService.UpdateAsync(id, dto, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/properties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _propertyService.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}