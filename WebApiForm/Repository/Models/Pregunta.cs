using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiForm.Repository.Models;

[Table("Pregunta")]
public partial class Pregunta
{
    [Key]
    [Column("cod_pregunta")]
    public int CodPregunta { get; set; }

    [Column("pregunta")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Pregunta1 { get; set; }

    [JsonIgnore]
    [InverseProperty("CodPreguntaNavigation")]
    public virtual ICollection<Sesion> Sesions { get; set; } = new List<Sesion>();
}
