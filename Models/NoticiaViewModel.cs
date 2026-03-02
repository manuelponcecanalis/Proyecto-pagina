namespace Pagina_proyecto.Models
{
    public class NoticiaViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Cuerpo { get; set; }
        public string PrimeraImagenUrl { get; set; } = string.Empty; // Valor por defecto para evitar nulos
        public bool EsCarrusel { get; set; } // Propiedad para el estado del carrusel

        // ✅ Lista de imágenes disponibles en la carpeta Imagenes_Anuncios
        public List<string> Imagenes { get; set; } = new List<string>();
    }
}
