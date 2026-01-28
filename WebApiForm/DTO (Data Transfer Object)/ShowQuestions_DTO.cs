public class Questions_DTO
{
    public int? IdSesion { get; set; }

    public string TipoRespuesta { get; set; } = null!;
    public string? GrupoTema { get; set; }

    public int CodPregunta { get; set; }
    public string? Pregunta { get; set; }

    public string? CodSubPregunta { get; set; }
    public string? SubPregunta { get; set; }

    public int Estado { get; set; }
    public string? Rango { get; set; }
}
