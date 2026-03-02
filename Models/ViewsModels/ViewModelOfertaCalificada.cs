using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pagina_proyecto.Models.Entities
{
    [Table("RecomendacionesFCE")]
    public class OfertaCalificada
    {
        [Key]
        public int ID { get; set; }


        public string? Materia { get; set; }
        public string? Docente { get; set; }
        public string? Sede { get; set; }

        [Column("Diasdecursada")]
        public string? DiasDeCursada { get; set; }

        [Column("Horariodecursada")]
        public string? HorarioDeCursada { get; set; }

        public string? Cuatrimestre { get; set; }

        [Column("Modalidaddecursada")]
        public string? ModalidadDeCursada { get; set; }

        public string? Puntaje { get; set; }

        [Column("Niveldelacursada")]
        public string? NivelDeLaCursada { get; set; }

        [Column("Dificultaddelcurso")]
        public string? DificultadDelCurso { get; set; }

        [Column("Calidaddelasclases")]
        public string? CalidadDeLasClases { get; set; }

        [Column("Formatodelasclases")]
        public string? FormatoDeLasClases { get; set; }

        [Column("Serespetoeldiavirtualasignado")]
        public string? SeRespetoElDiaVirtualAsignado { get; set; }

        [Column("Modalidaddelosparciales")]
        public string? ModalidadDeLosParciales { get; set; }

        [Column("Dificultaddelosparciales")]
        public string? DificultadDeLosParciales { get; set; }

        [Column("Recomendariaselcurso")]
        public string? RecomendariasElCurso { get; set; }

        [Column("InformacionAdicional")]
        public string? OtraInfo { get; set; }
    }
}
