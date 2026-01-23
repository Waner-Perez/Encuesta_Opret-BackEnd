using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiForm.Repository.Models;
using WebApiForm.Repository;

namespace WebApiForm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SesionsController : ControllerBase
    {
        private readonly FormEncuestaDbContext _context;

        public SesionsController(FormEncuestaDbContext context)
        {
            _context = context;
        }

        // GET: api/Sesions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sesion>>> GetSesions()
        {
            return await _context.Sesions.ToListAsync();
        }

        // GET: api/Sesions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sesion>> GetSesion(int id)
        {
            var sesion = await _context.Sesions.FindAsync(id);

            if (sesion == null)
            {
                return NotFound();
            }

            return sesion;
        }

        // PUT: api/Sesions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSesion(int id, Sesion sesion)
        {
            if (id != sesion.IdSesion)
            {
                return BadRequest("El ID en la URL no coincide con el ID en el cuerpo de la solicitud.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("El modelo de la sesión no es válido.");
            }

            _context.Entry(sesion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SesionExists(id))
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

        // POST: api/Sesions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Sesion>> PostSesion(Sesion sesion)
        {
            try
            {
                // Deshabilitar el seguimiento de cambios para permitir que el trigger maneje la clave primaria
                //_context.ChangeTracker.AutoDetectChangesEnabled = false;

                // Asegúrate de que IdSesion no tenga un valor asignado
                sesion.IdSesion = 0;

                _context.Sesions.Add(sesion);
                await _context.SaveChangesAsync();

                // Recuperar el valor generado por el trigger
                var newId = await _context.Sesions
                    .OrderByDescending(s => s.IdSesion)
                    .Select(s => s.IdSesion)
                    .FirstOrDefaultAsync();

                // Asignar el valor generado a la entidad
                sesion.IdSesion = newId;

                // Rehabilitar el seguimiento de cambios
                //_context.ChangeTracker.AutoDetectChangesEnabled = true;

                return CreatedAtAction("GetSesion", new { id = sesion.IdSesion }, sesion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear la Sesion", details = ex.Message });
            }
        }

        // DELETE: api/Sesions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSesion(int id)
        {
            var sesion = await _context.Sesions.FindAsync(id);
            if (sesion == null)
            {
                return NotFound();
            }

            _context.Sesions.Remove(sesion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SesionExists(int id)
        {
            return _context.Sesions.Any(e => e.IdSesion == id);
        }
    }
}
