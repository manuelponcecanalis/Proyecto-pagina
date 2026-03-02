
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using System.Security.Claims;

namespace Pagina_proyecto.Controllers.Academico
{
    [ApiController]
    [Route("api/carreras")]
    public class CarrerasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarrerasController(AppDbContext context)
        {
            _context = context;
        }

        /* =====================================================
           GET: api/carreras
           Lista simple (tu versión original)
        ===================================================== */
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var carreras = await _context.Carreras
                .AsNoTracking()
                .ToListAsync();

            return Ok(carreras);
        }


        /* =====================================================
           GET: api/carreras/con-materias
           Incluye materias (N-N)
        ===================================================== */
        [HttpGet("con-materias")]
        public async Task<IActionResult> GetConMaterias()
        {
            var carreras = await _context.Carreras
                .Include(c => c.CarrerasMateria)
                    .ThenInclude(cm => cm.Materia)
                .AsNoTracking()
                .ToListAsync();

            return Ok(carreras);
        }


        /* =====================================================
           GET: api/carreras/dto
           DTO limpio sin tabla intermedia
        ===================================================== */
        [HttpGet("dto")]
        public async Task<IActionResult> GetDTO()
        {
            var carreras = await _context.Carreras
                .Select(c => new
                {
                    c.IdCarrera,
                    c.Nombre,
                    Materias = c.CarrerasMateria
                        .Select(cm => new
                        {
                            cm.IdMateria,
                            cm.Materia.Nombre,
                            cm.Materia.Horas
                        })
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(carreras);
        }


        /* =====================================================
           GET: api/carreras/mis-carreras
           Carreras del usuario logueado
        ===================================================== */
        [HttpGet("mis-carreras")]
        public async Task<IActionResult> GetMisCarreras()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var carreras = await _context.UsuarioCarreras
                .Where(uc => uc.IdUsuario == userId && uc.Activa)
                .Select(uc => new
                {
                    uc.Carrera.IdCarrera,
                    uc.Carrera.Nombre,
                    uc.FechaInicio
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(carreras);
        }
    }
}

