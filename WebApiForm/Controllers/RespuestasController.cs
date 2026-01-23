using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForm.Capa_de_Servicio;
using WebApiForm.DTO__Data_Transfer_Object_;
using WebApiForm.Repository;
using WebApiForm.Repository.Models;

namespace WebApiForm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RespuestasController : ControllerBase
    {
        private readonly FormEncuestaDbContext _context;
        private readonly RespuestaService _respuestaService;

        public RespuestasController(FormEncuestaDbContext context, RespuestaService respuestaService)
        {
            _context = context;
            _respuestaService = respuestaService;
        }

        // GET: api/Respuestas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Respuesta>>> GetRespuestas()
        {
            return await _context.Respuestas.ToListAsync();
        }

        // GET: api/Respuestas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Respuesta>> GetRespuesta(int id)
        {
            var respuesta = await _context.Respuestas.FindAsync(id);

            if (respuesta == null)
            {
                return NotFound();
            }

            return respuesta;
        }

        // PUT: api/Respuestas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRespuesta(int id, Respuesta respuesta)
        {
            if (id != respuesta.IdRespuestas)
            {
                return BadRequest();
            }

            _context.Entry(respuesta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RespuestaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Respuestas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Respuesta>> PostRespuesta(Respuesta respuesta)
        {
            try
            {
                // Deshabilitar el seguimiento de cambios para permitir que el trigger maneje la clave primaria
                //_context.ChangeTracker.AutoDetectChangesEnabled = false;

                _context.Respuestas.Add(respuesta);
                await _context.SaveChangesAsync();

                // Rehabilitar el seguimiento de cambios
                //_context.ChangeTracker.AutoDetectChangesEnabled = true;

                return CreatedAtAction("GetRespuesta", new { id = respuesta.IdRespuestas }, respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear enviar la Respuesta", details = ex.Message });
            }
        }

        // DELETE: api/Respuestas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRespuesta(int id)
        {
            var respuesta = await _context.Respuestas.FindAsync(id);
            if (respuesta == null)
            {
                return NotFound();
            }

            _context.Respuestas.Remove(respuesta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RespuestaExists(int id)
        {
            return _context.Respuestas.Any(e => e.IdRespuestas == id);
        }

        [HttpPost("insertar")]
        public async Task<IActionResult> postInsertarRespuesta([FromBody] List<Respuesta_Dto> respuestas)
        {
            if (respuestas == null || !respuestas.Any()) { 
                return BadRequest(new { message = "El cuerpo de la solicitud debe ser un array de respuestas." }); 
            }

            //try
            //{
            //    foreach (var answer in respuestas)
            //    {
            //        await _respuestaService.InsertarRespuestaAsyncServices(answer);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(new { message = "Error al enviar la respuesta", details = ex.Message });
            //}

            
            var resultados = new List<object>(); // Lista para almacenar el estado de cada respuesta

            try{
                foreach (var answer in respuestas)
                {
                    try
                    {
                        await _respuestaService.InsertarRespuestaAsyncServices(answer);
                        resultados.Add(new { respuestas = answer, status = "success" });
                    }
                    catch(Exception ex)
                    {
                        resultados.Add(new { respuestas = answer, status = "error", message = ex.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error general al enviar la respuesta", details = ex.Message });
            }

            return Ok(resultados);
        }

        [HttpGet("ObtenerResp")]
        public async Task<ActionResult<List<ObtenerRespuestas_Dto>>> getObtenerRespuestas()
        {
            try
            {
                var answer = await _respuestaService.ObtenerRespuestasAsyncService();
                return Ok(answer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al obtener el Reporte de las Respuestas", details = ex.Message });
            }
        }
    }
}
