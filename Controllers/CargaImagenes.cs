using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace TuProyecto.Controllers
{
    public class ImagenesController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Subir(IFormFile imagen)
        {
            if (imagen != null && imagen.Length > 0)
            {
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Imagenes_Anuncios", imagen.FileName);
                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }
            }

            // Redirige a la página anterior
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}