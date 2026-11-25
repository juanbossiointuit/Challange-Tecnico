using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultasPlan.Data;
using ConsultasPlan.Models;
using ConsultasPlan.Helpers;

namespace ConsultasPlan.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConsultasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var consultas = await _context.Consultas
            .Include(c => c.Afiliado)
            .Include(c => c.Profesional)
            .ToListAsync();
        return Ok(consultas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var consulta = await _context.Consultas
            .Include(c => c.Afiliado)
            .Include(c => c.Profesional)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (consulta == null) return NotFound(new { mensaje = "Consulta no encontrada." });
        return Ok(consulta);
    }

    [HttpPost]
    public async Task<IActionResult> CrearConsulta([FromBody] Consulta consulta)
    {
        var afiliado = await _context.Afiliados.FindAsync(consulta.AfiliadoId);
        if (afiliado == null)
            return NotFound(new { mensaje = "Afiliado no encontrado." });

        if (afiliado.Bloqueado)
            return BadRequest(new { mensaje = "El afiliado se encuentra bloqueado para agendar consultas." });

        var profesionalExiste = await _context.Profesionales.AnyAsync(p => p.Id == consulta.ProfesionalId);
        if (!profesionalExiste)
            return NotFound(new { mensaje = "Profesional no encontrado." });

        var consultaConflicto = await _context.Consultas.AnyAsync(c =>
            c.ProfesionalId == consulta.ProfesionalId &&
            c.FechaHora == consulta.FechaHora &&
            c.Estado != EstadoConsulta.Cancelado);
        if (consultaConflicto)
            return BadRequest(new { mensaje = "El profesional ya tiene una consulta en ese horario." });

        consulta.FechaCreacion = DateTime.UtcNow;
        consulta.Estado = EstadoConsulta.Pendiente;
        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = consulta.Id }, consulta);
    }

    [HttpPost("cancelar/{id}")]
    public async Task<IActionResult> CancelarConsulta(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return NotFound(new { mensaje = "Consulta no encontrada." });

        if (consulta.FechaHora - DateTime.Now < TimeSpan.FromHours(23))
            return BadRequest(new { mensaje = "No se puede cancelar con menos de 24 horas de anticipación." });

        consulta.Estado = EstadoConsulta.Cancelado;
        await _context.SaveChangesAsync();
        return Ok(consulta);
    }

    [HttpPost("inasistencia/{id}")]
    public async Task<IActionResult> MarcarInasistencia(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return NotFound(new { mensaje = "Consulta no encontrada." });

        if (consulta.FechaHora > DateTime.Now)
            return BadRequest(new { mensaje = "No se puede registrar una inasistencia sobre una consulta futura." });

        consulta.Estado = EstadoConsulta.Inasistencia;
        await _context.SaveChangesAsync();
        return Ok(consulta);
    }

    [HttpPost("atencion/{id}")]
    public async Task<IActionResult> RegistrarAtencion(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return NotFound(new { mensaje = "Consulta no encontrada." });

        if (!DateTimeExtensions.IsInCurrentYear(consulta.FechaHora))
            return BadRequest(new { mensaje = "Solo se pueden registrar atenciones de consultas del año en curso." });

        consulta.Estado = EstadoConsulta.Atendido;
        await _context.SaveChangesAsync();
        return Ok(consulta);
    }

    [HttpPut("estado/{id}")]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoRequest request)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) NotFound(new { mensaje = "Consulta no encontrada." });

        consulta.Estado = request.Estado;
        await _context.SaveChangesAsync();
        return Ok(consulta);
    }
}
