using ConsultasPlan.Data;
using ConsultasPlan.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultasPlan.Services
{
    public class SedesService
    {
        private readonly AppDbContext _context;
        public SedesService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Sede>> GetSedes()
        {
            return await _context.Sedes.ToListAsync();
        }
    }
}
