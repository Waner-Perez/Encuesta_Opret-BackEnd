using Microsoft.EntityFrameworkCore;
using System.IO;
using WebApiForm.DTO__Data_Transfer_Object_;
using WebApiForm.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiForm.Capa_de_Servicio
{
    public class RespuestaService
    {
        private readonly FormularioRepository _context;

        // Inyectar el repositorio en el constructor
        public RespuestaService(FormularioRepository context)
        {
            _context = context;
        }

        // Método para insertar una respuesta
        public async Task InsertarRespuestaAsyncServices(Respuesta_Dto respuesta)
        {
            await _context.InsertarRespuestaAsync(respuesta);
        }

        // Método para obtener respuestas
        //Obtener todas las Respuesta de su tabla Respuestas mas el Usuario, Sesion, Preguntas, Subpreguntas, basado en un stored procedure
        public async Task<List<ObtenerRespuestas_Dto>> ObtenerRespuestasAsyncService()
        {
            return await _context.ObtenerRespuestasAsync();
        }

        public async Task<List<ReportRespuestas_Dto>> ExportReportRespuestaServices()
        {
            return await _context.GetReporteAsync();
        }
    }
}
