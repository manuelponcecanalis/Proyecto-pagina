using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pagina_proyecto.Models.Entities

{
    [Authorize(Roles = "Operador")]
    public class CargaCalendarioController : Controller
    {
        private readonly AppDbContext _context;

        public CargaCalendarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CargaCalendario
        public async Task<IActionResult> Index()
        {
              return _context.Calendario != null ? 
                          View(await _context.Calendario.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Calendario'  is null.");
        }

        // GET: CargaCalendario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Calendario == null)
            {
                return NotFound();
            }

            var cargaCalendario = await _context.Calendario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cargaCalendario == null)
            {
                return NotFound();
            }

            return View(cargaCalendario);
        }

        // GET: CargaCalendario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CargaCalendario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TituloEvento,CuerpoEvento,InicioEvento,FinEvento")] CargaCalendario cargaCalendario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cargaCalendario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cargaCalendario);
        }

        // GET: CargaCalendario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Calendario == null)
            {
                return NotFound();
            }

            var cargaCalendario = await _context.Calendario.FindAsync(id);
            if (cargaCalendario == null)
            {
                return NotFound();
            }
            return View(cargaCalendario);
        }

        // POST: CargaCalendario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TituloEvento,CuerpoEvento,InicioEvento,FinEvento")] CargaCalendario cargaCalendario)
        {
            if (id != cargaCalendario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cargaCalendario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CargaCalendarioExists(cargaCalendario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cargaCalendario);
        }

        // GET: CargaCalendario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Calendario == null)
            {
                return NotFound();
            }

            var cargaCalendario = await _context.Calendario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cargaCalendario == null)
            {
                return NotFound();
            }

            return View(cargaCalendario);
        }

        // POST: CargaCalendario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Calendario == null)
            {
                return Problem("Entity set 'AppDbContext.Calendario'  is null.");
            }
            var cargaCalendario = await _context.Calendario.FindAsync(id);
            if (cargaCalendario != null)
            {
                _context.Calendario.Remove(cargaCalendario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CargaCalendarioExists(int id)
        {
          return (_context.Calendario?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
