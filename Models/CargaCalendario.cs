namespace Pagina_proyecto.Models
{
    public class CargaCalendario
    {
       
            // Propiedades públicas para que puedan ser accedidas desde otras clases
            public int Id { get; set; }
            public string TituloEvento { get; set; }
            public string CuerpoEvento { get; set; }
            public DateTime InicioEvento { get; set; }
            public DateTime FinEvento { get; set; }

        
    }
}
