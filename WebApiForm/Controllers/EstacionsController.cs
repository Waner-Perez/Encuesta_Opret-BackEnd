using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForm.Capa_de_Servicio;
using WebApiForm.Repository;
using WebApiForm.Repository.Models;

namespace WebApiForm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstacionsController : ControllerBase
    {
        private readonly FormEncuestaDbContext _context;
        private readonly EstacionPorLineaService _estacionPorLineaService;

        public EstacionsController(FormEncuestaDbContext context, EstacionPorLineaService estacionPorLineaService)
        {
            _context = context;
            _estacionPorLineaService = estacionPorLineaService;
        }

        // GET: api/Estacions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estacion>>> GetEstacions()
        {
            return await _context.Estacions.ToListAsync();
        }

        // GET: api/Estacions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Estacion>> GetEstacion(int id)
        {
            var estacion = await _context.Estacions.FindAsync(id);

            if (estacion == null)
            {
                return NotFound();
            }

            return estacion;
        }

        // PUT: api/Estacions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstacion(int id, Estacion estacion)
        {
            if (id != estacion.IdEstacion)
            {
                return BadRequest();
            }

            _context.Entry(estacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstacionExists(id))
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

        // POST: api/Estacions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Estacion>> PostEstacion(Estacion estacion)
        {
            _context.Estacions.Add(estacion);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EstacionExists(estacion.IdEstacion))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEstacion", new { id = estacion.IdEstacion }, estacion);
        }

        // DELETE: api/Estacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstacion(int id)
        {
            try
            {
                var estacion = await _context.Estacions.FindAsync(id);
                if (estacion == null)
                {
                    return NotFound(new { message = "La Estación no fue encontrada." });
                }

                _context.Estacions.Remove(estacion);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null && dbEx.InnerException.Message.Contains("fk_Estacion_linea"))
                {
                    return BadRequest(new { message = "Lo sentimos, esta Estación de metro no se puede eliminar, porque, ya tiene un formulario de registro de los Empleados." });
                }

                return BadRequest(new { message = "Ocurrió un error en la base de datos", details = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado", details = ex.Message });
            }
        }

        private bool EstacionExists(int id)
        {
            return _context.Estacions.Any(e => e.IdEstacion == id);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("linea/{idLinea}")]
        public async Task<IActionResult> ObtenerEstacionesPorLinea(string idLinea)
        {
            var estaciones = await _estacionPorLineaService.ObtenerEstacionesPorLineaAsync(idLinea);

            if (estaciones == null || estaciones.Count == 0)
            {
                return NotFound(new { success = false, message = "No se encontraron estaciones para esta línea." });
            }

            return Ok(new { success = true, result = estaciones });
        }
    }
}
