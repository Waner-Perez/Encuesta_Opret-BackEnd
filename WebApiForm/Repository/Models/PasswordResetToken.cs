using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiForm.Repository.Models;

[Table("PasswordResetToken")]
public partial class PasswordResetToken
{
    [Key]
    [Column("token")]
    [StringLength(100)]
    [Unicode(false)]
    public string Token { get; set; } = null!;

    [Column("id_usuarios")]
    [StringLength(100)]
    [Unicode(false)]
    public string IdUsuarios { get; set; } = null!;

    [Column("expiration")]
    public DateTime Expiration { get; set; }

    [ForeignKey("IdUsuarios")]
    [InverseProperty("PasswordResetTokens")]
    public virtual RegistroUsuario? IdUsuariosNavigation { get; set; }
}
