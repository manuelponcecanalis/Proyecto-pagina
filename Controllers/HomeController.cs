using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Areas.Data;
using Pagina_proyecto.Models.Entities;
using Pagina_proyecto.Models.ViewModels;
using Pagina_proyecto.Models;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;




namespace Pagina_proyecto.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;
       
        private readonly AppDbContext _context;
        private readonly RecomendacionService _recomendacionService;

        public HomeController(
            ILogger<HomeController> logger,
            
            AppDbContext context,
            RecomendacionService recomendacionService)
        {
            _logger = logger;
            _context = context;
            _recomendacionService = recomendacionService;
        }

        public IActionResult ObtenerPlanesDeEstudio()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PlanesDeEstudio");

            if (!Directory.Exists(folderPath))
                return Json(new List<object>());

            var archivos = Directory.GetFiles(folderPath)
                .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".jpeg") || file.EndsWith(".webp"))
                .Select(file => new
                {
                    Titulo = Path.GetFileNameWithoutExtension(file),
                    Url = "/PlanesDeEstudio/" + Path.GetFileName(file)
                }).ToList();

            return Json(archivos);
        }

        public IActionResult ObtenerMapas()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "mapas");

            if (!Directory.Exists(folderPath))
                return Json(new List<object>());

            var imagenes = Directory.GetFiles(folderPath, "*.jpg")
                .Select(file => new
                {
                    Titulo = Path.GetFileNameWithoutExtension(file),
                    Url = "/mapas/" + Path.GetFileName(file)
                }).ToList();

            return Json(imagenes);
        }
        public async Task<IActionResult> Noticias()
        {
            var noticias = await _context.Noticias
                .Select(n => new NoticiaPreviewViewModel
                {
                    Id = n.Id,
                    Titulo = n.Titulo,
                    Subtitulo = n.Subtitulo,
                    PrimeraImagenUrl = n.Imagenes.FirstOrDefault().Url
                })
                .ToListAsync();

            return View("~/Views/Home/noticias.cshtml", noticias);
        }

        public async Task<IActionResult> Index()
        {
            // Noticias para el carrusel
            var noticias = await _context.Noticias
                .Select(n => new NoticiaPreviewViewModel
                {
                    Id = n.Id,
                    Titulo = n.Titulo,
                    Subtitulo = n.Subtitulo,
                    PrimeraImagenUrl = n.Imagenes.Select(i => i.Url).FirstOrDefault()
                })
                .ToListAsync();

            // Eventos futuros
            var eventos = await _context.Calendario
      .Where(e => e.FinEvento >= DateTime.Now) // ahora incluye hoy y m�s tarde
      .OrderBy(e => e.InicioEvento)
      .ToListAsync();


            var eventosVM = eventos.Select(e => new EventoCalendarioViewModel
            {
                Id = e.Id,
                Titulo = e.TituloEvento,
                Cuerpo = e.CuerpoEvento,
                Mes = e.InicioEvento.ToString("MMM", new CultureInfo("es-ES")).ToUpper(),
                Dia = e.InicioEvento.Day.ToString(),
                FechaInicio = e.InicioEvento.ToString("d MMMM", new CultureInfo("es-ES")),
                FechaFin = e.FinEvento.ToString("d MMMM", new CultureInfo("es-ES"))
            }).ToList();


            var viewModel = new HomeIndexViewModel
            {
                Noticias = noticias,
                EventosCalendario = eventosVM
            };

            return View("~/Views/Home/index.cshtml", viewModel);
        }

        public IActionResult Conocenos()
        {
            ViewData["Title"] = "Nosotros - PROYECTO Económicas";
            return View();
        }

        /// <summary>Contenido migrado desde Pagina_proyecto_economicas/carreras.html</summary>
        public IActionResult Carreras()
        {
            ViewData["Title"] = "Carreras";
            return View();
        }

        /// <summary>Contenido migrado desde Pagina_proyecto_economicas/calendario-academico.html</summary>
        public IActionResult CalendarioAcademico()
        {
            ViewData["Title"] = "Calendario acad�mico";
            return View();
        }

        /// <summary>Contenido migrado desde Pagina_proyecto_economicas/inscripcion-materias.html (secci�n modalidades)</summary>
        public IActionResult ModalidadesCursada()
        {
            ViewData["Title"] = "Modalidades de cursada";
            return View();
        }

        /// <summary>Resumen orientativo de normas de cursada (consult� siempre la normativa oficial en FCE UBA).</summary>
        public IActionResult ReglamentoCursada()
        {
            ViewData["Title"] = "Reglamento de cursada";
            return View();
        }

        /// <summary>Informaci�n general sobre becas; enlaces a sitios oficiales UBA.</summary>
        public IActionResult Becas()
        {
            ViewData["Title"] = "Becas";
            return View();
        }

        /// <summary>Contenido migrado desde Pagina_proyecto_economicas/inscripcion-materias.html</summary>
        public IActionResult InscripcionMaterias()
        {
            ViewData["Title"] = "Inscripci�n a materias";
            return View();
        }

        /// <summary>Contenido migrado desde Pagina_proyecto_economicas/certificados.html</summary>
        public IActionResult Certificados()
        {
            ViewData["Title"] = "Certificados";
            return View();
        }

        /// <summary>Contenido migrado desde Pagina_proyecto_economicas/titulo-en-tramite.html</summary>
        public IActionResult TituloEnTramite()
        {
            ViewData["Title"] = "T�tulo en tr�mite";
            return View();
        }

        /// <summary>�ndice de tr�mites y enlaces �tiles (MiEcon, Departamento de Alumnos).</summary>
        public IActionResult GuiaTramites()
        {
            ViewData["Title"] = "Gu�a de tr�mites";
            return View();
        }

        public async Task<IActionResult> BiblioEconomicas()
        {
            // Trae solo materias activas
            var materiasActivas = await _context.MateriasDrive
                                        .Where(m => m.Activo)
                                        .OrderByDescending(m => m.FechaCreacion)
                                        .ToListAsync();

            var viewModel = new HomeIndexViewModel
            {
                Materias = materiasActivas
            };

            return View(viewModel);
        }



        [Authorize(Roles = "Operador")]
        public IActionResult Actividades()
        {
            return View();
        }

        public IActionResult RecomendacionesFCE()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MostrarDatos()
        {
            try
            {
                var datos = _recomendacionService.ObtenerTodas();

                var opcionesJson = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null // Mantiene los nombres tal como est�n en el modelo
                };

                return new JsonResult(datos, opcionesJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        public IActionResult CBC()
        {
            ViewData["Title"] = "Mundo CBC - PROYECTO Econ�micas";
            return View();
        }

        /// <summary>Contenido migrado desde la gu�a CBC (inscripciones y calendario).</summary>
        public IActionResult CbcInscripciones()
        {
            ViewData["Title"] = "Inscripciones CBC";
            return View();
        }

        /// <summary>Sedes del CBC (Ciudad Universitaria, Medrano, Drago).</summary>
        public IActionResult CbcSedes()
        {
            ViewData["Title"] = "Sedes CBC";
            return View();
        }

        /// <summary>Grupos de WhatsApp para ingresantes y cursada CBC.</summary>
        public IActionResult CbcGruposWhatsApp()
        {
            ViewData["Title"] = "Grupos de WhatsApp - CBC";
            return View();
        }

        public IActionResult Ranking()
        {
            return View();
        }
      

        [HttpGet]
        public async Task<IActionResult> LoadMoreNoticias(int pageNumber, int pageSize)
        {
            var noticias = await _context.Noticias
                .Include(n => n.Imagenes)
                .OrderByDescending(n => n.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NoticiaPreviewViewModel
                {
                    Id = n.Id,
                    Titulo = n.Titulo,
                    Subtitulo = n.Subtitulo,
                    PrimeraImagenUrl = n.Imagenes.FirstOrDefault().Url
                })
                .ToListAsync();

            return PartialView("_NoticiasPartial", noticias);
        }

        public async Task<IActionResult> DetalleNoticia(int id)
        {
            var noticia = await _context.Noticias
                .Include(n => n.Imagenes)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (noticia == null)
            {
                return NotFound();
            }

            var viewModel = new NoticiaPreviewViewModel
            {
                Id = noticia.Id,
                Titulo = noticia.Titulo,
                Subtitulo = noticia.Subtitulo,
                PrimeraImagenUrl = noticia.Imagenes.FirstOrDefault()?.Url,
                Cuerpo = noticia.Cuerpo
            };

            return View("~/Views/Home/DetalleNoticia.cshtml", viewModel);
        }

        [Authorize(Roles = "Operador")]
        public IActionResult Sistemas()
        {
            return View("~/Views/Materias/Sistemas.cshtml");
        }


    }
}
