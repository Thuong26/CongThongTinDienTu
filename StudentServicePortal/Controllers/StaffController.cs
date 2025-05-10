using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;

[ApiController]
[Authorize(Roles = "Manager,Staff")]
[Route("api/staff")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly IRegistrationFormService _formService;
    private readonly IRegistrationDetailService _registrationDetailService;
    private readonly IFormService _formService2;
    private readonly IRegulationService _regulationService;
    public StaffController(IStaffService staffService, IRegistrationFormService formService, IRegistrationDetailService registrationDetailService, IRegulationService regulationService, IFormService formService2)
    {
        _staffService = staffService;
        _formService = formService;
        _registrationDetailService = registrationDetailService;
        _regulationService = regulationService;
        _formService2 = formService2;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<Staff>> GetProfile()
    {
        string maCB = "CB0001"; // Hardcoded để test

        var staff = await _staffService.GetProfileAsync(maCB);

        if (staff == null)
        {
            return NotFound(new { message = "No officer found." });
        }

        return Ok(staff);
    }
    [HttpGet("forms")]
    public async Task<IActionResult> GetPendingForms()
    {
        var forms = await _formService.GetPendingFormsAsync();
        return Ok(forms);
    }
    [HttpGet("forms/{maDon}")]
    public async Task<IActionResult> GetRegistrationDetails(string maDon)
    {
        var details = await _registrationDetailService.GetDetailsByFormIdAsync(maDon);
        if (!details.Any())
        {
            return NotFound();
        }
        return Ok(details);
    }
    [HttpPut("forms/{maDon}/status")]
    public async Task<IActionResult> UpdateFormStatus(string maDon, [FromBody] UpdateFormStatusRequest request)
    {
        var success = await _registrationDetailService.UpdateStatusByMaDonAsync(maDon, request.TrangThaiXuLy);

        if (!success)
            return NotFound(new { message = $"Không tìm thấy đơn có mã {maDon}" });

        return Ok(new { message = "Cập nhật trạng thái thành công." });
    }

    [HttpPost("templates")]
    public async Task<IActionResult> CreateForm([FromBody] Form form)
    {
        if (form == null)
            return BadRequest("Biểu mẫu không được rỗng");

        var success = await _formService2.CreateFormAsync(form);

        if (success)
            return Ok(new { message = "Tạo biểu mẫu thành công" });

        return StatusCode(500, new { message = "Tạo biểu mẫu thất bại" });
    }
    [HttpPut("templates/{maBM}")]
    public async Task<IActionResult> UpdateForm(string maBM, [FromBody] Form form)
    {
        var success = await _formService2.UpdateFormAsync(maBM, form);
        if (!success)
            return NotFound(new { message = "Biểu mẫu không tồn tại hoặc cập nhật thất bại." });

        return Ok(new { message = "Cập nhật biểu mẫu thành công." });
    }
    [HttpPost]
    public async Task<IActionResult> CreateRegulation([FromBody] Regulation regulation)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _regulationService.CreateRegulationAsync(regulation);
        if (!result)
            return StatusCode(500, new { message = "Tạo quy định thất bại." });

        return Ok(new { message = "Tạo quy định thành công." });
    }
    [HttpPut("{maQD}")]
    public async Task<IActionResult> UpdateRegulation(string maQD, [FromBody] Regulation regulation)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _regulationService.UpdateRegulationAsync(maQD, regulation);
        if (!result)
            return NotFound(new { message = "Không tìm thấy hoặc cập nhật thất bại." });

        return Ok(new { message = "Cập nhật quy định thành công." });
    }
}
