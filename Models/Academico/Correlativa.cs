namespace Pagina_proyecto.Models.Entities
{
    public class Correlativa
    {
        public int IdMateria { get; set; }                 // FK hacia Materias
        public int IdMateriaCorrelativa { get; set; }      // FK hacia Materias

        // Relaciones de navegación
        public Materia Materia { get; set; } = null!;
        public Materia MateriaCorrelativa { get; set; } = null!;
    }
}
