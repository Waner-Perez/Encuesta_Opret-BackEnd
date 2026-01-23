using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiForm.Repository.Models;

[Table("Linea")]
public partial class Linea
{
    [Key]
    [Column("id_linea")]
    [StringLength(20)]
    [Unicode(false)]
    public string IdLinea { get; set; } = null!;

    [Column("tipo_linea")]
    [StringLength(50)]
    [Unicode(false)]
    public string TipoLinea { get; set; } = null!;

    [Column("nombre_linea")]
    [StringLength(255)]
    [Unicode(false)]
    public string? NombreLinea { get; set; }

    [JsonIgnore]
    [InverseProperty("IdLineaNavigation")]
    public virtual ICollection<Estacion> Estacions { get; set; } = new List<Estacion>();

    [JsonIgnore]
    [InverseProperty("IdLineaNavigation")]
    public virtual ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();
}
