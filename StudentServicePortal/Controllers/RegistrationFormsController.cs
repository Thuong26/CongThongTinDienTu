using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Controllers
{
    [ApiController]
    [Route("api/students/forms")]
    public class RegistrationFormsController : ControllerBase
    {
        private readonly IRegistrationFormService _formService;
        private readonly IRegistrationDetailService _service;
        public RegistrationFormsController(IRegistrationFormService formService, IRegistrationDetailService service)
        {
            _formService = formService;
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistrationForm>>> GetAllForms()
        {
            var forms = await _formService.GetAllForms();
            return Ok(forms);
        }
        [HttpGet("details/{maDon}")]
        public async Task<ActionResult<RegistrationDetail>> GetFormById(string maDon)
        {
            var form = await _formService.GetFormById(maDon);
            if (form == null)
                return NotFound($"No application found with code: {maDon}");

            return Ok(form);
        }
        [HttpGet("{maDon}")]
        public async Task<ActionResult<RegistrationForm>> GetByFormIdAsync(string maDon)
        {
            var form = await _formService.GetByFormIdAsync(maDon);
            if (form == null)
                return NotFound($"No application found with code: {maDon}");

            return Ok(form);
        }
        [HttpPost]
        public async Task<IActionResult> AddForm([FromBody] RegistrationForm form)
        {
            if (form == null || string.IsNullOrEmpty(form.MaDon))
            {
                return BadRequest("Invalid data.");
            }

            form.ThoiGianDang = form.ThoiGianDang == default ? DateTime.Now : form.ThoiGianDang;
            form.TrangThai = true;

            await _formService.AddForm(form);

            return CreatedAtAction(nameof(GetFormById), new { maDon = form.MaDon }, form);
        }
        [HttpPost("detail")]
        public async Task<IActionResult> Create([FromBody] RegistrationDetail detail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(detail);
            return CreatedAtAction(nameof(GetById), new { maDonCT = created.MaDonCT }, created);
        }
        [HttpGet("{maDonCT}")]
        public async Task<IActionResult> GetById(string maDonCT)
        {
            var detail = await _service.GetByIdAsync(maDonCT);
            if (detail == null)
                return NotFound();
            return Ok(detail);
        }
    }
}
