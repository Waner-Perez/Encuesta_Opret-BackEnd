using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiForm.Repository.Models;

[Table("Sesion")]
public partial class Sesion
{
    [Key]
    [Column("id_sesion")]
    public int? IdSesion { get; set; }

    [Column("tipo_respuesta")]
    [StringLength(100)]
    [Unicode(false)]
    public string TipoRespuesta { get; set; } = null!;

    [Column("grupo_tema")]
    [StringLength(100)]
    [Unicode(false)]
    public string? GrupoTema { get; set; }

    [Column("cod_pregunta")]
    public int CodPregunta { get; set; }

    [Column("cod_subPregunta")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CodSubPregunta { get; set; }

    [Column("rango")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Rango { get; set; }

    [Column("estado")]
    public bool Estado { get; set; }

    [JsonIgnore]
    [ForeignKey("CodPregunta")]
    [InverseProperty("Sesions")]
    public virtual Pregunta? CodPreguntaNavigation { get; set; }

    [JsonIgnore]
    [ForeignKey("CodSubPregunta")]
    [InverseProperty("Sesions")]
    public virtual SubPregunta? CodSubPreguntaNavigation { get; set; }

    [JsonIgnore]
    [InverseProperty("IdSesionNavigation")]
    public virtual ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
}
