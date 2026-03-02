using Microsoft.AspNetCore.Identity;
using Pagina_proyecto.Models.Entities;

namespace Pagina_proyecto.Areas.Data
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public int NumeroRegistro { get; set; }

        /* 🔗 Relación N-N con Carreras */
        public ICollection<UsuarioCarrera> UsuarioCarreras { get; set; }
            = new List<UsuarioCarrera>();
    }
}