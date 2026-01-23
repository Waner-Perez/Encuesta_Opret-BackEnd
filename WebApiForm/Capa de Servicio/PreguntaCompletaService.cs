using WebApiForm.Repository;
using WebApiForm.Services;

namespace WebApiForm.Capa_de_Servicio
{
    public class PreguntaCompletaService
    {
        private readonly FormularioRepository _context;

        public PreguntaCompletaService(FormularioRepository context)
        {
            _context = context;
        }

        public async Task<List<PreguntaCompleta>> ObtenerPreguntasCompletoAsync()
        {
            return await _context.GetPreguntasCompleto();
        }
    }
}
