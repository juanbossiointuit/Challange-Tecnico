using ConsultasPlan.Data;
using ConsultasPlan.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultasPlan.Services
{
    public class ConsultasService
    {
        private readonly AppDbContext _context;
        private readonly AfiliadosService _afiliadosService;
        
        public ConsultasService(AppDbContext context, AfiliadosService afiliadosService)
        {
            _context = context;
            _afiliadosService = afiliadosService;
        }
        public async Task<List<Consulta>> GetConsultas()
        {
            var consultas = await _context.Consultas.Include(c => c.Afiliado).Include(c => c.Profesional).ToListAsync();
            return consultas;
        }
        public async Task<Consulta?> GetById(int id)
        {
            var consulta = await _context.Consultas.Include(c => c.Afiliado).Include(c => c.Profesional).FirstOrDefaultAsync(c => c.Id == id);
            return consulta;
        }
        public async Task<ConsultaDTO> CrearConsulta(Consulta consulta)
        {
            var afiliado = await _afiliadosService.GetAfiliadoById(consulta.AfiliadoId);
            if (afiliado == null)
                return new ConsultaDTO(null,  "Afiliado no encontrado.", false);

            if (afiliado.Bloqueado)
                return new ConsultaDTO(null, "El afiliado se encuentra bloqueado para agendar consultas.", true);
            
            var profesionalExiste = await _context.Profesionales.AnyAsync(p => p.Id == consulta.ProfesionalId);
            if (!profesionalExiste)
                return new ConsultaDTO(null, "Profesional no encontrado.", false);

            var consultaConflicto = await _context.Consultas.AnyAsync(c =>
                c.ProfesionalId == consulta.ProfesionalId &&
                c.FechaHora == consulta.FechaHora &&
                c.Estado != EstadoConsulta.Cancelado);
            if (consultaConflicto)
                return new ConsultaDTO(null, "El profesional ya tiene una consulta en ese horario.", false);

            consulta.FechaCreacion = DateTime.UtcNow;
            consulta.Estado = EstadoConsulta.Pendiente;
            _context.Consultas.Add(consulta);
            return new ConsultaDTO(consulta, null, true);
        }
        public async Task<ConsultaDTO> CancelarConsulta(int id)
        {
            var consulta = await GetById(id);
            if (consulta == null) return new ConsultaDTO(null, "Consulta no encontrada.", false);

            if (consulta.FechaHora - DateTime.Now < TimeSpan.FromHours(24))
                return new ConsultaDTO(null, "No se puede cancelar con menos de 24 horas de anticipación.", true);

            consulta.Estado = EstadoConsulta.Cancelado;
            await _context.SaveChangesAsync();
            return new ConsultaDTO(consulta, null, true);
        }
        public async Task<ConsultaDTO> MarcarInasistencia(int id)
        {
            var consulta = await GetById(id);
            if (consulta == null) return new ConsultaDTO(null, "Consulta no encontrada.", false);

            if (consulta.FechaHora > DateTime.Now)
                return new ConsultaDTO(null, "No se puede registrar una inasistencia sobre una consulta futura.", true);

            consulta.Estado = EstadoConsulta.Inasistencia;
            await _context.SaveChangesAsync();
            return new ConsultaDTO(consulta, null, true);
        }
        public async Task<ConsultaDTO> RegistrarAtencion(int id)
        {
            var consulta = await GetById(id);
            if (consulta == null) return new ConsultaDTO(null, "Consulta no encontrada.", false);

            if (consulta.FechaHora > DateTime.Now)
                return new ConsultaDTO(null, "Solo se pueden registrar atenciones de consultas del año en curso.", true);

            consulta.Estado = EstadoConsulta.Atendido;
            await _context.SaveChangesAsync();
            return new ConsultaDTO(consulta, null, true);
        }
        public async Task<ConsultaDTO> ActualizarEstado(int id, ActualizarEstadoRequest request)
        {
            var consulta = await GetById(id);
            if (consulta == null) return new ConsultaDTO(null, "Consulta no encontrada.", false);

            if (consulta.FechaHora > DateTime.Now)
                return new ConsultaDTO(null, "Solo se pueden registrar atenciones de consultas del año en curso.", true);

            consulta.Estado = request.Estado;
            await _context.SaveChangesAsync();
            return new ConsultaDTO(consulta, null, true);
        }
    }
}
