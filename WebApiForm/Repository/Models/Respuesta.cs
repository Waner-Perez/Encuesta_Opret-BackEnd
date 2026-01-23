using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiForm.Repository.Models;

public partial class Respuesta
{
    [Key]
    [Column("id_respuestas")]
    public int? IdRespuestas { get; set; }

    [Column("id_usuarios")]
    [StringLength(100)]
    [Unicode(false)]
    public string IdUsuarios { get; set; } = null!;

    [Column("no_encuesta")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NoEncuesta { get; set; }

    [Column("id_sesion")]
    public int IdSesion { get; set; }

    [Column("respuesta")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Respuesta1 { get; set; }

    [Column("comentarios")]
    [Unicode(false)]
    public string? Comentarios { get; set; }

    [Column("justificacion")]
    [Unicode(false)]
    public string? Justificacion { get; set; }

    [Column("hora_respuestas")]
    [StringLength(100)]
    [Unicode(false)]
    public string? HoraRespuestas { get; set; }

    [Column("identifacador_form")]
    public int? IdentifacadorForm { get; set; }

    [Column("fecha_respuestas")]
    [StringLength(100)]
    [Unicode(false)]
    public string? FechaRespuestas { get; set; }

    [JsonIgnore]
    [ForeignKey("IdSesion")]
    [InverseProperty("Respuestas")]
    public virtual Sesion? IdSesionNavigation { get; set; }

    [JsonIgnore]
    [ForeignKey("IdUsuarios")]
    [InverseProperty("Respuestas")]
    public virtual RegistroUsuario? IdUsuariosNavigation { get; set; }

    [JsonIgnore]
    [ForeignKey("IdentifacadorForm")]
    [InverseProperty("Respuestas")]
    public virtual Formulario? IdentifacadorFormNavigation { get; set; }
}
