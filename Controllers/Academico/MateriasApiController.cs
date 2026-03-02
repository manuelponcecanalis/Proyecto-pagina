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
                               x.IdCarrera == idCarrera &&
                               x.IdMateria == idMateria);

            if (yaAprobada)
                return BadRequest("La materia ya está aprobada");

            var correlativas = await _context.Correlativas
                .Where(c => c.IdMateria == idMateria)
                .Select(c => c.IdMateriaCorrelativa)
                .ToListAsync();

            var aprobadas = await _context.MateriasAprobadas
                .Where(a => a.IdUsuario == userId && a.IdCarrera == idCarrera)
                .Select(a => a.IdMateria)
                .ToListAsync();

            if (!correlativas.All(c => aprobadas.Contains(c)))
                return BadRequest("No se cumplen las correlativas");

            _context.MateriasAprobadas.Add(new MateriaAprobadaAlumno
            {
                IdUsuario = userId,
                IdCarrera = idCarrera,
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
                .Where(a => a.IdUsuario == userId && a.IdCarrera == idCarrera)
                .Select(a => a.IdMateria)
                .ToListAsync();

            var bloquearia = await _context.Correlativas
                .AnyAsync(c => c.IdMateriaCorrelativa == idMateria &&
                               aprobadas.Contains(c.IdMateria));

            if (bloquearia)
                return BadRequest("No podés desaprobar esta materia porque otras aprobadas dependen de ella");

            var aprobada = await _context.MateriasAprobadas
                .FirstOrDefaultAsync(x => x.IdUsuario == userId &&
                                          x.IdCarrera == idCarrera &&
                                          x.IdMateria == idMateria);

            if (aprobada == null) return NotFound();

            _context.MateriasAprobadas.Remove(aprobada);
            await _context.SaveChangesAsync();
            return Ok();
        }

        
    }
}