using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForm.Repository;
using WebApiForm.Repository.Models;

namespace WebApiForm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntasController : ControllerBase
    {
        private readonly FormEncuestaDbContext _context;

        public PreguntasController(FormEncuestaDbContext context)
        {
            _context = context;
        }

        // GET: api/Preguntas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pregunta>>> GetPreguntas()
        {
            return await _context.Preguntas.ToListAsync();
        }

        // GET: api/Preguntas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pregunta>> GetPregunta(int id)
        {
            var pregunta = await _context.Preguntas.FindAsync(id);

            if (pregunta == null)
            {
                return NotFound();
            }

            return pregunta;
        }

        // PUT: api/Preguntas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPregunta(int id, Pregunta pregunta)
        {
            if (id != pregunta.CodPregunta)
            {
                return BadRequest();
            }

            _context.Entry(pregunta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PreguntaExists(id))
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

        // POST: api/Preguntas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pregunta>> PostPregunta(Pregunta pregunta)
        {
            _context.Preguntas.Add(pregunta);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PreguntaExists(pregunta.CodPregunta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPregunta", new { id = pregunta.CodPregunta }, pregunta);
        }

        // DELETE: api/Preguntas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePregunta(int id)
        {
            try
            {
                var pregunta = await _context.Preguntas.FindAsync(id);
                if (pregunta == null)
                {
                    return NotFound();
                }

                _context.Preguntas.Remove(pregunta);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                if(dbEx.InnerException != null && dbEx.InnerException.Message.Contains("fk_Sesion_Pregunta"))
                {
                    return BadRequest(new { message = "Esta pregunta esta siendo utilizado en la tabla Sesión. SI quieres eliminarlo deberas de borrarlo primero en la tabla de Sesión" });
                }
                
                return BadRequest(new { message = "Ocurrió un error en la base de datos", details = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado", details = ex.Message });
            }
        }

        private bool PreguntaExists(int id)
        {
            return _context.Preguntas.Any(e => e.CodPregunta == id);
        }
    }
}
