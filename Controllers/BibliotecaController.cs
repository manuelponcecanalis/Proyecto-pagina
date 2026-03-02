using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using System.Threading.Tasks;


public class BibliotecaController : Controller
{
    private readonly AppDbContext
 _context;

    public BibliotecaController(AppDbContext
 context)
    {
        _context = context;
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var materia = await _context.MateriasDrive.FindAsync(id);
        if (materia == null) return NotFound();

        materia.CantidadVisualizaciones++;
        await _context.SaveChangesAsync();

        return Redirect(materia.LinkCarpeta);
    }
}
