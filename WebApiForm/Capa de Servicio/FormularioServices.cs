using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using WebApiForm.DTO__Data_Transfer_Object_;
using WebApiForm.Repository;

namespace WebApiForm.Capa_de_Servicio
{
    public class FormularioServices
    {
        private readonly FormularioRepository _context;

        public FormularioServices(FormularioRepository context)
        {
            _context = context;
        }

        public async Task<List<ObtenerForm_Dto>> ObtenerFormularioAsyncService()
        {
            return await _context.ObtenerFormularioAsync();
        }
    }
}
