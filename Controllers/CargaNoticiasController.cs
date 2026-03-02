using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
namespace Pagina_proyecto.Models.Entities { 


[Authorize(Roles = "Operador")]

public class CargaNoticiasController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CargaNoticiasController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> EditarNoticia(int id)
    {
        var noticia = await _context.Noticias
            .Include(n => n.Imagenes)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (noticia == null)
        {
            return NotFound();
        }

        var model = new NoticiaViewModel
        {
            Id = noticia.Id,
            Titulo = noticia.Titulo,
            Subtitulo = noticia.Subtitulo,
            Cuerpo = noticia.Cuerpo,
            PrimeraImagenUrl = noticia.Imagenes?.FirstOrDefault()?.Url,
            EsCarrusel = noticia.EsCarrusel,
            Imagenes = Directory.GetFiles(Path.Combine(_env.WebRootPath, "Imagenes_Anuncios"))
                                .Select(Path.GetFileName)
                                .ToList()
        };

        return View("EditarNoticia", model);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(NoticiaViewModel model)
    {
        if (ModelState.IsValid)
        {
            var noticia = await _context.Noticias
                .Include(n => n.Imagenes)
                .FirstOrDefaultAsync(n => n.Id == model.Id);

            if (noticia == null)
            {
                return NotFound();
            }

            noticia.Titulo = model.Titulo;
            noticia.Subtitulo = model.Subtitulo;
            noticia.Cuerpo = model.Cuerpo;
            noticia.EsCarrusel = model.EsCarrusel;

            var imagen = noticia.Imagenes.FirstOrDefault();
            if (imagen != null)
            {
                imagen.Url = model.PrimeraImagenUrl;
            }

            _context.Update(noticia);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Recargar imágenes si hay error
        model.Imagenes = Directory.GetFiles(Path.Combine(_env.WebRootPath, "Imagenes_Anuncios"))
                                  .Select(Path.GetFileName)
                                  .ToList();

        return View(model);
    }

    public IActionResult CrearNoticia()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearNoticia(NoticiaViewModel model)
    {
        if (ModelState.IsValid)
        {
            var noticia = new CargaNoticia
            {
                Titulo = model.Titulo,
                Subtitulo = model.Subtitulo,
                Cuerpo = model.Cuerpo,
                EsCarrusel = model.EsCarrusel,
                Imagenes = new List<ImagenesEnNoticias>
                {
                    new ImagenesEnNoticias { Url = model.PrimeraImagenUrl }
                }
            };

            _context.Noticias.Add(noticia);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // ✅ Método para subir nueva imagen a la carpeta wwwroot/Imagenes_Anuncios
    [HttpPost]
    public async Task<IActionResult> SubirImagen(IFormFile nuevaImagen, int id)
    {
        if (nuevaImagen != null && nuevaImagen.Length > 0)
        {
            var ruta = Path.Combine(_env.WebRootPath, "Imagenes_Anuncios", nuevaImagen.FileName);
            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await nuevaImagen.CopyToAsync(stream);
            }
        }

        return RedirectToAction("EditarNoticia", new { id = id });
    }

    public IActionResult Index()
    {
        var noticias = _context.Noticias.ToList();
        return View(noticias);
    }
}
}