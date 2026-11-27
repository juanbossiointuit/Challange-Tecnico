using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultasPlan.Data;
using ConsultasPlan.Services;

namespace ConsultasPlan.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfesionalesController : ControllerBase
{
    private readonly ProfesionalesService _service;

    public ProfesionalesController(ProfesionalesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var profesionales = await _service.GetProfesionales();
        return Ok(profesionales);
    }
}
