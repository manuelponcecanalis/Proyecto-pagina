using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using Pagina_proyecto.Models.Entities;

namespace Pagina_proyecto.Controllers.Academico
{
    [ApiController]
    [Authorize]
    [Route("api/materias")]
    public class MateriasApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MateriasApiController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST api/materias/aprobar?idMateria=1&idCarrera=2
        [HttpPost("aprobar")]
        public async Task<IActionResult> AprobarMateria(int idMateria, int idCarrera)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var yaAprobada = await _context.MateriasAprobadas
                .AnyAsync(x => x.IdUsuario == userId &&
                               x.IdMateria == idMateria);
            if (yaAprobada)
                return BadRequest("La materia ya está aprobada");

            var correlativas = await _context.Correlativas
                .Where(c => c.IdCarrera == idCarrera && c.IdMateria == idMateria)
                .Select(c => c.IdMateriaCorrelativa)
                .ToListAsync();

            var aprobadas = await _context.MateriasAprobadas
                .Where(a => a.IdUsuario == userId)
                .Select(a => a.IdMateria)
                .ToListAsync();

            // Correlativas virtuales (id < 31): necesitás tantas aprobadas como el id
            // Correlativas reales (id >= 31): necesitás tener esa materia aprobada
            bool cumpleCorrelativas = correlativas.All(c =>
                c < 31 ? aprobadas.Count >= c : aprobadas.Contains(c));

            if (!cumpleCorrelativas)
                return BadRequest("No se cumplen las correlativas");

            _context.MateriasAprobadas.Add(new MateriaAprobadaAlumno
            {
                IdUsuario = userId,
                IdMateria = idMateria
            });
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/materias/desaprobar?idMateria=1&idCarrera=2
        [HttpDelete("desaprobar")]
        public async Task<IActionResult> DesaprobarMateria(int idMateria, int idCarrera)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var aprobadas = await _context.MateriasAprobadas
                .Where(a => a.IdUsuario == userId)
                .Select(a => a.IdMateria)
                .ToListAsync();

            // Solo verificar bloqueo para correlativas reales (id >= 31)
            var bloquearia = await _context.Correlativas
                .AnyAsync(c => c.IdCarrera == idCarrera &&
                               c.IdMateriaCorrelativa == idMateria &&
                               c.IdMateriaCorrelativa >= 31 &&
                               aprobadas.Contains(c.IdMateria));

            if (bloquearia)
                return BadRequest("No podés desaprobar esta materia porque otras aprobadas dependen de ella");

            var aprobada = await _context.MateriasAprobadas
                .FirstOrDefaultAsync(x => x.IdUsuario == userId &&
                                          x.IdMateria == idMateria);
            if (aprobada == null) return NotFound();

            _context.MateriasAprobadas.Remove(aprobada);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}