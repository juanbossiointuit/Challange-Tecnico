using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultasPlan.Data;
using ConsultasPlan.Models;
using ConsultasPlan.Helpers;
using ConsultasPlan.Services;

namespace ConsultasPlan.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly ConsultasService _service;

    public ConsultasController(ConsultasService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var consultas = await _service.GetConsultas();
        return Ok(consultas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var consulta = await _service.GetById(id);
        return Ok(consulta);
    }

    [HttpPost]
    public async Task<IActionResult> CrearConsulta([FromBody] Consulta consulta)
    {
        var consultaDTO = await _service.CrearConsulta(consulta);
        if (!consultaDTO.encontrado) return NotFound(new { mensaje = consultaDTO.mensajeError });
        if (!string.IsNullOrEmpty(consultaDTO.mensajeError)) return BadRequest(new { mensaje = consultaDTO.mensajeError });

        return Ok(consultaDTO.consulta);
    }

    [HttpPost("cancelar/{id}")]
    public async Task<IActionResult> CancelarConsulta(int id)
    {
        var consultaDTO = await _service.CancelarConsulta(id);
        if (!consultaDTO.encontrado) return NotFound(new { mensaje = consultaDTO.mensajeError });
        if (!string.IsNullOrEmpty(consultaDTO.mensajeError)) return BadRequest(new { mensaje = consultaDTO.mensajeError });

        return Ok(consultaDTO.consulta);
    }

    [HttpPost("inasistencia/{id}")]
    public async Task<IActionResult> MarcarInasistencia(int id)
    {
        var consultaDTO = await _service.MarcarInasistencia(id);
        if (!consultaDTO.encontrado) return NotFound(new { mensaje = consultaDTO.mensajeError });
        if (!string.IsNullOrEmpty(consultaDTO.mensajeError)) return BadRequest(new { mensaje = consultaDTO.mensajeError });

        return Ok(consultaDTO.consulta);
    }

    [HttpPost("atencion/{id}")]
    public async Task<IActionResult> RegistrarAtencion(int id)
    {
        var consultaDTO = await _service.RegistrarAtencion(id);
        if (!consultaDTO.encontrado) return NotFound(new { mensaje = consultaDTO.mensajeError });
        if (!string.IsNullOrEmpty(consultaDTO.mensajeError)) return BadRequest(new { mensaje = consultaDTO.mensajeError });

        return Ok(consultaDTO.consulta);
    }

    [HttpPut("estado/{id}")]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoRequest request)
    {
        var consultaDTO = await _service.ActualizarEstado(id, request);
        if (!consultaDTO.encontrado) return NotFound(new { mensaje = consultaDTO.mensajeError });
        if (!string.IsNullOrEmpty(consultaDTO.mensajeError)) return BadRequest(new { mensaje = consultaDTO.mensajeError });

        return Ok(consultaDTO.consulta);
    }
}
