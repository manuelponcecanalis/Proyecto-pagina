namespace Pagina_proyecto.Models.Entities
{
    public class CarrerasMateria
    {
        public int IdCarrera { get; set; }
        public int IdMateria { get; set; }

        /* Navegaciones */
        public Carrera Carrera { get; set; } = null!;
        public Materia Materia { get; set; } = null!;
    }
}