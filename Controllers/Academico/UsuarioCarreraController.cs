
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using Pagina_proyecto.Models.Entities;

namespace Pagina_proyecto.Controllers.Academico
{
    [Authorize]
    [ApiController]
    [Route("api/usuario-carrera")]
    public class UsuarioCarreraController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UsuarioCarreraController(
            AppDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /* =====================================================
           INSCRIBIRSE A CARRERA
        ===================================================== */
        [HttpPost("{idCarrera:int}")]
        public async Task<IActionResult> Inscribirse(int idCarrera)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            /* 🔹 Verificar que la carrera exista */
            var carreraExiste = await _context.Carreras
                .AnyAsync(c => c.IdCarrera == idCarrera);

            if (!carreraExiste)
                return NotFound("La carrera no existe");

            /* 🔹 ¿Ya existe relación? */
            var relacion = await _context.UsuarioCarreras
                .FirstOrDefaultAsync(x =>
                    x.IdUsuario == userId &&
                    x.IdCarrera == idCarrera);

            if (relacion != null)
            {
                if (relacion.Activa)
                    return BadRequest("Ya estás inscripto en esta carrera");

                /* 🔄 Reactivar inscripción */
                relacion.Activa = true;
                relacion.FechaInicio = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok("Inscripción reactivada");
            }

            /* 🔹 Nueva inscripción */
            _context.UsuarioCarreras.Add(new UsuarioCarrera
            {
                IdUsuario = userId,
                IdCarrera = idCarrera,
                Activa = true,
                FechaInicio = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok("Inscripción realizada");
        }
    }
}

