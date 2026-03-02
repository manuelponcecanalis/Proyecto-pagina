public class CargaNoticia
{
    public int Id { get; set; }  // Identificador único de la noticia
    public string Titulo { get; set; }  // Título de la noticia
    public string Subtitulo { get; set; }  // Subtítulo de la noticia
    public string Cuerpo { get; set; }  // Cuerpo de la noticia
    public bool EsCarrusel { get; set; }  // Indica si la noticia aparece en el carrusel
    public List<ImagenesEnNoticias> Imagenes { get; set; }  // Lista de imágenes relacionadas con la noticia
}

