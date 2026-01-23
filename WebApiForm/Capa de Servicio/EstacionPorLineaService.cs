using WebApiForm.Repository;
using WebApiForm.Services.DTO__Data_Transfer_Object_;

namespace WebApiForm.Capa_de_Servicio
{
    public class EstacionPorLineaService
    {
        private readonly FormularioRepository _context;

        public EstacionPorLineaService(FormularioRepository context)
        {
            _context = context;
        }

        public async Task<List<EstacionPorLinea>> ObtenerEstacionesPorLineaAsync(string idLinea)
        {
            return await _context.GetEstacionPorLineas(idLinea);
        }
    }
}
