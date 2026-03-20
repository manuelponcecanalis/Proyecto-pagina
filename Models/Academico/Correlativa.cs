namespace Pagina_proyecto.Models.Entities
{
    public class Correlativa
    {
        public int IdCarrera { get; set; }
        public int IdMateria { get; set; }
        public int IdMateriaCorrelativa { get; set; }

        // Relaciones de navegación
        public Carrera Carrera { get; set; } = null!;
        public Materia Materia { get; set; } = null!;
        public Materia MateriaCorrelativa { get; set; } = null!;
    }
}