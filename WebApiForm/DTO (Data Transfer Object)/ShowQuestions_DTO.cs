public class Questions_DTO
{
    public int? IdSesion { get; set; }
    public string TipoRespuesta { get; set; } = null!;
    public string? GrupoTema { get; set; }
    public string codPregunta { get; set; } = null!;
    public string? Pregunta { get; set; }
    public string codSubPregunta { get; set; } = null!;
    public string? SubPregunta { get; set; }
    public int Estado { get; set; } 
    public string? Rango { get; set; }
}
