using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultasPlan.Data;
using ConsultasPlan.Services;

namespace ConsultasPlan.Controllers;

[ApiController]
[Route("[controller]")]
public class SedesController : ControllerBase
{
    private readonly SedesService _service;

    public SedesController(SedesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sedes = await _service.GetSedes();
        return Ok(sedes);
    }
}
