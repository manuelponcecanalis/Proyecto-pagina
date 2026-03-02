namespace Pagina_proyecto.Models.Entities
{
    public class Materia
    {
        public int IdMateria { get; set; }

        // Coincide con la columna de la base de datos
        public string Nombre { get; set; } = null!;

        public int Horas { get; set; }

        /* 🔗 Relación N-N con Carreras */
        public ICollection<CarrerasMateria> CarrerasMateria { get; set; }
            = new List<CarrerasMateria>();


        /* 🔗 Correlativas */

        // Materias que ESTA materia necesita para cursarse
        public ICollection<Correlativa> Correlativas { get; set; }
            = new List<Correlativa>();

        // Materias que DEPENDEN de esta materia
        public ICollection<Correlativa> EsCorrelativaDe { get; set; }
            = new List<Correlativa>();
    }
}