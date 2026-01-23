using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForm.Repository;
using WebApiForm.Repository.Models;

namespace WebApiForm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineasController : ControllerBase
    {
        private readonly FormEncuestaDbContext _context;

        public LineasController(FormEncuestaDbContext context)
        {
            _context = context;
        }

        // GET: api/Lineas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Linea>>> GetLineas()
        {
            return await _context.Lineas.ToListAsync();
        }

        // GET: api/Lineas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Linea>> GetLinea(string id)
        {
            var linea = await _context.Lineas.FindAsync(id);

            if (linea == null)
            {
                return NotFound();
            }

            return linea;
        }

        // PUT: api/Lineas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLinea(string id, Linea linea)
        {
            if (id != linea.IdLinea)
            {
                return BadRequest();
            }

            _context.Entry(linea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineaExists(id))
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

        // POST: api/Lineas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Linea>> PostLinea(Linea linea)
        {
            _context.Lineas.Add(linea);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LineaExists(linea.IdLinea))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLinea", new { id = linea.IdLinea }, linea);
        }

        // DELETE: api/Lineas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLinea(string id)
        {
            try
            {
                var linea = await _context.Lineas.FindAsync(id);
                if (linea == null)
                {
                    return NotFound(new { message = "La línea no fue encontrada." });
                }

                _context.Lineas.Remove(linea);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                if(dbEx.InnerException != null && dbEx.InnerException.Message.Contains("fk_Estacion_linea"))
                {
                    return BadRequest(new { message = "No se puede borrar esta línea del metro porque hay estaciones que esta utilizando esta linea, para porder borrar esta linea primero deberas de borrar las estaciones que estan vinculados con esta." });
                }

                return BadRequest(new { message = "Ocurrió un error en la base de datos", details = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado", details = ex.Message });
            }
        }

        private bool LineaExists(string id)
        {
            return _context.Lineas.Any(e => e.IdLinea == id);
        }
    }
}
