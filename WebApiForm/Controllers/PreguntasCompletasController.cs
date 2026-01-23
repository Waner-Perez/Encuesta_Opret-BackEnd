using Microsoft.AspNetCore.Mvc;
using WebApiForm.Capa_de_Servicio;
using WebApiForm.Services;

namespace WebApiForm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntasCompletasController : ControllerBase
    {
        private readonly PreguntaCompletaService _preguntaCompletaService;

        public PreguntasCompletasController(PreguntaCompletaService preguntaCompletaService)
        {
            _preguntaCompletaService = preguntaCompletaService;
        }

        [HttpGet("obtenerQuestion")]
        public async Task<ActionResult<IEnumerable<PreguntaCompleta>>> ObtenerPreguntasCompleto()
        {
            try
            {
                var preguntasCompleto = await _preguntaCompletaService.ObtenerPreguntasCompletoAsync();
                if (preguntasCompleto == null || preguntasCompleto.Count == 0)
                {
                    return StatusCode(404, "No se encontraron preguntas completas.");
                }
                return Ok(preguntasCompleto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error interno al procesar la solicitud. Por favor, inténtelo de nuevo más tarde.");
            }
        }
    }
}
