using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using Pagina_proyecto.Models.Entities;

namespace Pagina_proyecto.Controllers.Academico
{
    [Authorize]
    public class MateriasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MateriasController(
            AppDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Sistemas()
        {
            return View();
        }

        public async Task<IActionResult> Cronograma(int? idCarrera)
        {
            var userId = _userManager.GetUserId(User);

            ViewBag.Carreras = await _context.Carreras.ToListAsync();

            if (idCarrera == null)
            {
                var materiasTodas = await _context.Materias
                    .Include(m => m.Correlativas)
                    .ToListAsync();

                ViewBag.Aprobadas = new List<int>();
                return View(materiasTodas);
            }

            var materias = await _context.CarrerasMateria
                .Where(cm => cm.IdCarrera == idCarrera)
                .Include(cm => cm.Materia)
                    .ThenInclude(m => m.Correlativas)
                .Select(cm => cm.Materia)
                .Where(m => m != null)
                .ToListAsync();

            var aprobadas = await _context.MateriasAprobadas
                .Where(a => a.IdUsuario == userId &&
                            a.IdCarrera == idCarrera)
                .Select(a => a.IdMateria)
                .ToListAsync();

            ViewBag.Aprobadas = aprobadas;
            ViewBag.IdCarrera = idCarrera;

            return View(materias);
        }

       
    }
}