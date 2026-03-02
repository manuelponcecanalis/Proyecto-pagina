using Pagina_proyecto.Models;
using System.Collections.Generic;

namespace Pagina_proyecto.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<MateriasDrive> Materias { get; set; } = new List<MateriasDrive>();

        // Otros campos que ya tenías
        public List<NoticiaPreviewViewModel> Noticias { get; set; } = new List<NoticiaPreviewViewModel>();
        public List<EventoCalendarioViewModel> EventosCalendario { get; set; } = new List<EventoCalendarioViewModel>();
    }
}
