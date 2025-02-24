using Microsoft.AspNetCore.Mvc;
using AadharUpdateAPI.Models;
using AadharUpdateAPI.Services;


namespace AadharUpdateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AadharController : ControllerBase
    {
        private readonly AadharService _service;

        public AadharController()
        {
            _service = new AadharService();
        }

        // Get all Aadhar records
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _service.GetAll();
            return Ok(data);
        }

        // Get an Aadhar record by Id
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var data = _service.GetById(id);
            return data == null ? NotFound($"No record found for Id {id}") : Ok(data);
        }

        // Create a new Aadhar record
        [HttpPost]
        public IActionResult Create([FromBody] AadharDetails aadhar)
        {
            if (aadhar == null)
                return BadRequest("Invalid Aadhar details.");

            var createdAadhar = _service.Add(aadhar);
            return CreatedAtAction(nameof(GetById), new { id = createdAadhar.Id }, createdAadhar);
        }

        // Update an existing Aadhar record by Id
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AadharDetails aadhar)
        {
            if (aadhar == null || aadhar.Id != id)
                return BadRequest("Invalid Aadhar details or mismatched Id.");

            var updated = _service.Update(id, aadhar);
            return updated ? NoContent() : NotFound($"No record found for Id {id}");
        }

        // Delete an Aadhar record by Id
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _service.Delete(id);
            return deleted ? NoContent() : NotFound($"No record found for Id {id}");
        }
    }
}