using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pagina_proyecto.Models
{
    [Table("MateriasDrive")]
    public partial class MateriasDrive
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la materia es obligatorio")]
        [StringLength(200)]
        public string Materia { get; set; } = null!;

        [Required(ErrorMessage = "La descripción del contenido es obligatoria")]
        public string DescripcionContenido { get; set; } = null!;

        public int CantidadArchivos { get; set; } = 0;

        public int CantidadVisualizaciones { get; set; } = 0;

        [Required(ErrorMessage = "El link de la carpeta es obligatorio")]
        [Url(ErrorMessage = "Debe ser un link válido")]
        public string LinkCarpeta { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;
    }
}
