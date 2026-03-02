using Pagina_proyecto.Areas.Data;

namespace Pagina_proyecto.Models.Entities
{
    public class UsuarioCarrera
    {
        public string IdUsuario { get; set; } = null!;
        public AppUser Usuario { get; set; } = null!;

        public int IdCarrera { get; set; }
        public Carrera Carrera { get; set; } = null!;

        public bool Activa { get; set; }
        public DateTime FechaInicio { get; set; }
    }
}
