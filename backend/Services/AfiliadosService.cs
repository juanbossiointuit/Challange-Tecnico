using ConsultasPlan.Data;
using ConsultasPlan.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultasPlan.Services
{
    public class AfiliadosService
    {
        private readonly AppDbContext _context;
        public AfiliadosService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Afiliado>> GetAfiliados()
        {
            var afiliados = await _context.Afiliados.ToListAsync();
            return afiliados;
        }
        public async Task<Afiliado?> GetAfiliadoById(int id)
        {
            var afiliado = await _context.Afiliados.FindAsync(id);
            return afiliado;
        }
        public async Task<Afiliado> CreateAfiliado(Afiliado afiliado)
        {
            afiliado.createdAt = DateTime.UtcNow;
            afiliado.isActive = true;
            _context.Afiliados.Add(afiliado);
            await _context.SaveChangesAsync();
            return afiliado;
        }
        public async Task<Afiliado?> UpdateAfiliado(int id, Afiliado afiliado)
        {
            var afiliadoExistente = await _context.Afiliados.FindAsync(id);
            if (afiliadoExistente == null) return null;

            afiliadoExistente.NombreCompleto = afiliado.NombreCompleto;
            afiliadoExistente.DNI = afiliado.DNI;
            afiliadoExistente.Email = afiliado.Email;
            afiliadoExistente.Telefono = afiliado.Telefono;
            await _context.SaveChangesAsync();
            return afiliadoExistente;
        }
        public async Task<bool> DeleteAfiliado(int id)
        {
            var afiliado = await _context.Afiliados.FindAsync(id);
            if (afiliado == null) return false;

            _context.Afiliados.Remove(afiliado);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
