namespace Pagina_proyecto.Models.Entities
{
    public class Carrera
    {
        public int IdCarrera { get; set; }

        public string Nombre { get; set; } = null!;

        /* 🔗 Relación N-N con Materias */
        public ICollection<CarrerasMateria> CarrerasMateria { get; set; }
            = new List<CarrerasMateria>();

        /* 🔗 Relación con Usuarios (sin cambios) */
        public ICollection<UsuarioCarrera> Usuarios { get; set; }
            = new List<UsuarioCarrera>();
    }
}