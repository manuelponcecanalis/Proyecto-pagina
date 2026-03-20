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

        private static readonly Dictionary<string, int> _slugsCarrera = new()
        {
            { "contador", 1 },
            { "administracion", 2 },
            { "economia", 3 },
            { "sistemas", 4 },
            { "actuario", 5 },
            { "tec-datos", 6 }
        };

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

        [AllowAnonymous]
        public async Task<IActionResult> Cronograma(string? idCarrera)
        {
            int? idCarreraInt = null;

            if (!string.IsNullOrEmpty(idCarrera))
            {
                if (_slugsCarrera.TryGetValue(idCarrera, out var id))
                    idCarreraInt = id;
                else if (int.TryParse(idCarrera, out var idNum))
                    idCarreraInt = idNum;
            }

            var userId = _userManager.GetUserId(User);
            ViewBag.Carreras = await _context.Carreras.ToListAsync();

            if (idCarreraInt == null)
            {
                var materiasTodas = await _context.Materias
                    .Include(m => m.Correlativas)
                    .ToListAsync();
                ViewBag.Aprobadas = new List<int>();
                return View(materiasTodas);
            }

            var materias = await _context.CarrerasMateria
                .Where(cm => cm.IdCarrera == idCarreraInt)
                .Include(cm => cm.Materia)
                    .ThenInclude(m => m.Correlativas.Where(c => c.IdCarrera == idCarreraInt))
                .Include(cm => cm.Materia)
                    .ThenInclude(m => m.Correlativas)
                        .ThenInclude(c => c.MateriaCorrelativa)
                .Select(cm => cm.Materia)
                .Where(m => m != null)
                .ToListAsync();

            var ordenadas = OrdenarPorProfundidad(materias);

            var aprobadas = await _context.MateriasAprobadas
                .Where(a => a.IdUsuario == userId)
                .Select(a => a.IdMateria)
                .ToListAsync();

            ViewBag.Aprobadas = aprobadas;
            ViewBag.IdCarrera = idCarreraInt;
            return View(ordenadas);
        }

        private List<Materia> OrdenarPorProfundidad(List<Materia> materias)
        {
            var ids = materias.Select(m => m.IdMateria).ToHashSet();
            var profundidad = new Dictionary<int, int>();

            foreach (var m in materias)
            {
                profundidad[m.IdMateria] = CalcularProfundidad(m, materias, ids, new HashSet<int>());
            }

            return materias
                .OrderBy(m =>
                {
                    if (m.IdMateria >= 1001 && m.IdMateria <= 1008)
                        return int.MaxValue - 1;

                    var maxVirtual = m.Correlativas?
                        .Where(c => c.IdMateriaCorrelativa < 31)
                        .Select(c => c.IdMateriaCorrelativa)
                        .DefaultIfEmpty(0)
                        .Max() ?? 0;

                    if (maxVirtual > 20)
                        return int.MaxValue;

                    return profundidad[m.IdMateria];
                })
                .ThenBy(m => m.Correlativas?.Count ?? 0)
                .ThenBy(m => m.IdMateria)
                .ToList();
        }

        private int CalcularProfundidad(Materia materia, List<Materia> todas, HashSet<int> idsCarrera, HashSet<int> visitados)
        {
            if (visitados.Contains(materia.IdMateria))
                return 0;

            var correlativas = materia.Correlativas?
                .Select(c => c.IdMateriaCorrelativa)
                .Where(id => id >= 31 && idsCarrera.Contains(id))
                .ToList() ?? new List<int>();

            if (!correlativas.Any())
                return 0;

            visitados.Add(materia.IdMateria);

            int maxProfundidad = 0;
            foreach (var corrId in correlativas)
            {
                var corrMateria = todas.FirstOrDefault(m => m.IdMateria == corrId);
                if (corrMateria != null)
                {
                    var prof = CalcularProfundidad(corrMateria, todas, idsCarrera, new HashSet<int>(visitados));
                    if (prof > maxProfundidad)
                        maxProfundidad = prof;
                }
            }

            return maxProfundidad + 1;
        }
    }
}