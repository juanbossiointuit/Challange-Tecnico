using ConsultasPlan.Data;
using ConsultasPlan.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultasPlan.Services
{
    public class ProfesionalesService
    {
        private readonly AppDbContext _context;
        public ProfesionalesService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Profesional>> GetProfesionales()
        {
            return await _context.Profesionales.Include(p => p.Sede).ToListAsync();
        }
    }
}
