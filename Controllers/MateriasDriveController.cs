using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using Pagina_proyecto.Models;
using Pagina_proyecto.Models.Entities;
using Pagina_proyecto.Services; // 👈 ESTE FALTABA

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pagina_proyecto.Controllers
{
    [Authorize(Roles = "Operador")]
    public class MateriasDriveController : Controller
    {
        private readonly AppDbContext _context;
        private readonly GoogleDriveService _driveService;

        public MateriasDriveController(
            AppDbContext context,
            GoogleDriveService driveService)
        {
            _context = context;
            _driveService = driveService;
        }

        // GET: MateriasDrive
        public async Task<IActionResult> Index(string filtro)
        {
            var materias = _context.MateriasDrive.AsQueryable();

            switch (filtro)
            {
                case "Activas":
                    materias = materias.Where(m => m.Activo == true);
                    break;
                case "Inactivas":
                    materias = materias.Where(m => m.Activo == false);
                    break;
                default:
                    // "Todas" o null → no filtramos
                    break;
            }

            ViewData["Filtro"] = filtro ?? "Todas";

            return View(await materias.ToListAsync());
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MateriasDrive model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CantidadArchivos =
                await _driveService.ContarArchivosAsync(model.LinkCarpeta);

            model.CantidadVisualizaciones = 0;
            model.FechaCreacion = DateTime.Now;
            model.Activo = true;

            _context.MateriasDrive.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // DELETE: MateriasDrive/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var materia = await _context.MateriasDrive.FindAsync(id);
            if (materia == null)
                return NotFound();

            _context.MateriasDrive.Remove(materia);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Materia eliminada correctamente" });
        }

        // GET: MateriasDrive/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var materia = await _context.MateriasDrive.FindAsync(id);
            if (materia == null) return NotFound();

            return View(materia);
        }
        // POST: MateriasDrive/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MateriasDrive model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MateriasDriveExists(model.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MateriasDriveExists(int id)
        {
            return _context.MateriasDrive.Any(e => e.Id == id);
        }

        // POST: MateriasDrive/SincronizarArchivos
        [HttpPost]
        public async Task<IActionResult> SincronizarArchivos()
        {
            var materias = await _context.MateriasDrive
                .Where(m => m.Activo == true)
                .ToListAsync();

            foreach (var materia in materias)
            {
                materia.CantidadArchivos = await _driveService.ContarArchivosAsync(materia.LinkCarpeta);
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = $"Se sincronizaron {materias.Count} materias correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [Route("MateriasDrive/RegistrarVisualizacion")]
        public async Task<IActionResult> RegistrarVisualizacion([FromBody] int id)
        {
            var materia = await _context.MateriasDrive.FindAsync(id);
            if (materia == null)
                return NotFound();

            materia.CantidadVisualizaciones++;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }



}
