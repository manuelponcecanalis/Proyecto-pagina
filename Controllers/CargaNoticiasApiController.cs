using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;

namespace Pagina_proyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargaNoticiasApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CargaNoticiasApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoticia(int id)
        {
            var noticia = await _context.Noticias
                .Include(n => n.Imagenes)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (noticia == null)
            {
                return NotFound(new { message = "Noticia no encontrada." });
            }

            _context.Noticias.Remove(noticia);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Noticia eliminada correctamente." });
        }


        [HttpDelete("imagen/{nombre}")]
        public IActionResult EliminarImagen(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                return BadRequest(new { message = "Nombre de imagen inválido." });
            }

            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imagenes_Anuncios", nombre);

            if (!System.IO.File.Exists(ruta))
            {
                return NotFound(new { message = "Imagen no encontrada." });
            }

            System.IO.File.Delete(ruta);
            return Ok(new { message = "Imagen eliminada correctamente." });
        }

    }
}
