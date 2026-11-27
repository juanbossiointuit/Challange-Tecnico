namespace ConsultasPlan.Models
{
    public class ConsultaDTO
    {
        public ConsultaDTO(Consulta? _consulta, string? _mensajeError, bool _encontrado)
        {
            consulta = _consulta;
            mensajeError = _mensajeError;
            encontrado = _encontrado;
        }
        public Consulta? consulta { get; set; }
        public string? mensajeError { get; set; }
        public bool encontrado { get; set; }
        
    }
}
