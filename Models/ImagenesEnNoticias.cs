public class ImagenesEnNoticias
{
    public int Id { get; set; }  // Identificador único de la imagen
    public int NoticiaId { get; set; }  // Identificador de la noticia a la que pertenece la imagen
    public string Url { get; set; }  // URL de la imagen

    public CargaNoticia CargaNoticia { get; set; }  // Referencia a la noticia
}
