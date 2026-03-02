public class EventoCalendarioViewModel
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Cuerpo { get; set; }  // <--- nuevo
    public string Mes { get; set; }
    public string Dia { get; set; }
    public string FechaInicio { get; set; }
    public string FechaFin { get; set; }
}
