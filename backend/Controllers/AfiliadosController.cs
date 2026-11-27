using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultasPlan.Data;
using ConsultasPlan.Models;
using ConsultasPlan.Services;

namespace ConsultasPlan.Controllers;

[ApiController]
[Route("[controller]")]
public class AfiliadosController : ControllerBase
{
    private readonly AfiliadosService _service;

    public AfiliadosController(AfiliadosService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var afiliados = await _service.GetAfiliados();
        return Ok(afiliados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var afiliado = await _service.GetAfiliadoById(id);
        if (afiliado == null) return NotFound(new { mensaje = "Afiliado no encontrado." });
        return Ok(afiliado);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Afiliado afiliado)
    {
        var afiliadoCreado = await _service.CreateAfiliado(afiliado);
        return CreatedAtAction(nameof(GetById), new { id = afiliadoCreado.Id }, afiliadoCreado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Afiliado afiliado)
    {
        var afiliadoActualizado = await _service.UpdateAfiliado(id, afiliado);
        if (afiliadoActualizado == null) return NotFound(new { mensaje = "Afiliado no encontrado." });
        return Ok(afiliadoActualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resultadoEliminado = await _service.DeleteAfiliado(id);
        if (!resultadoEliminado) return NotFound(new { mensaje = "Afiliado no encontrado." });
        return NoContent();
    }
}
