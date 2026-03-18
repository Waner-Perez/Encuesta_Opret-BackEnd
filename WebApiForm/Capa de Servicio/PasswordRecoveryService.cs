using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebApiForm.Capa_de_Servicio.Encrypt;
using WebApiForm.Interfaces;
using WebApiForm.Repository;
using WebApiForm.Repository.Models;

namespace WebApiForm.Capa_de_Servicio
{
    public class PasswordRecoveryService
    {
        private readonly IEmailSender _emailSender;
        private readonly FormEncuestaDbContext _context;

        public PasswordRecoveryService(IEmailSender emailSender, FormEncuestaDbContext context)
        {
            _emailSender = emailSender;
            _context = context;
        }

        public string GenerarTokenRecuperacion()
        {
            return Guid.NewGuid().ToString();
        }

        private static string RecoveryCode(int size = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var code = new char[size];
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[1];

            for (int i = 0; i < size; i++)
            {
                do
                {
                    rng.GetBytes(buffer);
                } while (buffer[0] >= chars.Length * (byte.MaxValue / chars.Length));

                code[i] = chars[buffer[0] % chars.Length];
            }

            return new string(code);
        }

        public async Task<bool> CrearTokenRecuperacionAsync(string email)
        {
            var user = await _context.RegistroUsuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.IdUsuarios == null)
            {
                throw new Exception("Usuario no encontrado");
            }
            var token = RecoveryCode();
            var expiration = DateTime.UtcNow.AddHours(1);

            var passwordResetToken = new PasswordResetToken
            {
                IdUsuarios = user.IdUsuarios,
                Token = token,
                Expiration = expiration
            };

            _context.PasswordResetTokens.Add(passwordResetToken);
            await _context.SaveChangesAsync();

            // Enlace del endpoint "reset"
            string resetLink = $"{token}";
            string fullName = user.NombreApellido;
            string userName = user.Usuario;

            string subject = "Recuperación de Contraseña";
            
            string htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9; color: #333;'>
                    <h2 style='text-align: center; color: #667db6;'>✨ ¡Recupera tu Contraseña! ✨</h2>
                    <p style='font-size: 16px; line-height: 1.6;'>
                        <strong>Nombre de Usuario: </strong>'{userName}'. <br><br>
                        Hola <strong>{fullName}</strong>, hemos recibido una solicitud para restablecer tu contraseña vinculada a este correo. Si no has solicitado esto, puedes ignorar este mensaje.
                    </p>
                    <hr>
                    <p style='text-align: center; font-size: 16px;'>
                        <strong>No responder a este correo.</strong>
                    </p>
                    <hr>
                    <p style='font-size: 14px; text-align: center; color: #888;'>
                        Copia y pega el siguiente enlace en tu navegador:
                    </p>
                    <div style='background-color: #f1f1f1; padding: 10px; border: 1px dashed #ccc; border-radius: 5px; text-align: center;'>
                        <span id='resetLink' style='word-break: break-all; font-size: 14px; color: #555;'>{resetLink}</span>
                    </div>
                    <p style='font-size: 12px; text-align: center; margin-top: 20px; color: #888;'>
                        ⚠ Este enlace es válido por 1 hora. Si expira, solicita un nuevo enlace desde la App en la pestaña de recuperación de contraseña.
                    </p>
                    <p style='text-align: center;'>
                        <strong>💡 Consejo:</strong> Mantén presionado el enlace y selecciona <i>Copiar</i> en tu móvil o haz clic derecho en tu computadora.
                    </p>
                </div>
            ";

            await _emailSender.SendEmail(email, subject, htmlContent);

            return true;
        }

        public async Task<bool> VerificarTokenAsync(string token, string nuevaContraseña)
        {
            var passwordResetToken = await _context.PasswordResetTokens
                .Include(t => t.IdUsuariosNavigation)
                .FirstOrDefaultAsync(t => t.Token == token && t.Expiration > DateTime.UtcNow);

            if (passwordResetToken == null)
            {
                return false;
            }

            var user = passwordResetToken.IdUsuariosNavigation;

            // Generar nuevo salt y hashear la nueva contraseña
            string newSalt = SaltHelper.GenerateSalt();
            //user.Passwords = HashHelper.Hash(nuevaContraseña, newSalt);
            string hashedPassword = HashHelper.Hash(nuevaContraseña, newSalt);

            // Guarda el nuevo salt y hash en el formato esperado
            user.Passwords = $"{newSalt}:{hashedPassword}";

            _context.PasswordResetTokens.Remove(passwordResetToken); // Elimina el token una vez usado
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
