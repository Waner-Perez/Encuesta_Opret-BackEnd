using Microsoft.EntityFrameworkCore;
using WebApiForm.DTO__Data_Transfer_Object_;
using WebApiForm.Services;
using WebApiForm.Services.DTO__Data_Transfer_Object_;

namespace WebApiForm.Repository
{
    public class FormularioRepository
    {
        private readonly FormEncuestaDbContext _context;

        public FormularioRepository(FormEncuestaDbContext context)
        {
            _context = context;
        }

        public async Task<List<PreguntaCompleta>> GetPreguntasCompleto()
        {
            return await _context.PreguntaCompletas
                .FromSqlRaw("EXEC sp_ObtenerPreguntasCompleto")
                .ToListAsync();
        }

        public async Task<List<EstacionPorLinea>> GetEstacionPorLineas(string idLinea)
        {
            return await _context.EstacionPorLineas
                .FromSqlRaw("EXEC sp_ObternerEstacionesPorLinea @idLinea = {0}", idLinea)
                .ToListAsync();
        }

        public async Task<List<ObtenerForm_Dto>> ObtenerFormularioAsync()
        {
            return await _context.obtenerFormDtos
                .FromSqlRaw("EXEC sp_ObtenerForm_Linea_Estacion")
                .ToListAsync();
        }

        public async Task<List<ObtenerRespuestas_Dto>> ObtenerRespuestasAsync()
        {
            return await _context.obtenerRespuestasDtos
                .FromSqlRaw("EXEC sp_ObtenerRespuestas")
                .ToListAsync();
        }

        public async Task InsertarRespuestaAsync(Respuesta_Dto respuesta_Dto) => await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_InsertarRespuesta " +
                "@idUsuarios = {0}," +
                "@idSesion = {1}, " +
                "@respuesta = {2}, " +
                "@comentarios = {3}, " +
                "@justificacion = {4}, " +
                "@horaRespuestas = {5}," +
                "@fechaRespuestas = {6}," +
                "@finalizarSesion = {7}",
            respuesta_Dto.IdUsuarios,
            respuesta_Dto.IdSesion,
            respuesta_Dto.Respuesta,
            respuesta_Dto.Comentarios,
            respuesta_Dto.Justificacion,
            respuesta_Dto.HoraResp,
            respuesta_Dto.FechaResp,
            respuesta_Dto.FinalizarSesion
        );

        public async Task<List<ReportRespuestas_Dto>> GetReporteAsync() 
        {
            return await _context.reportRespuestas_Dtos
                .FromSqlRaw("EXEC sp_Report_Respuestas")
                .ToListAsync();
        }
    }
}
