using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiForm.Capa_de_Servicio.Encrypt;
using WebApiForm.DTO__Data_Transfer_Object_;
using WebApiForm.Interfaces;
using WebApiForm.Middleware;
using WebApiForm.Repository;
using WebApiForm.Repository.Models;
using WebApiForm.Services.Modelos_Tokens;

namespace WebApiForm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly FormEncuestaDbContext _context;
        //private readonly IEmailSender _emailSender;

        public LoginController(IConfiguration config, FormEncuestaDbContext context/*, IEmailSender emailSender*/)
        {
            _config = config;
            _context = context;
            //_emailSender = emailSender;
        }

        [HttpPost]
        [Route("User")]
        public async Task<IActionResult> InicioSesion([FromBody] Login_Model login)
        {
            string user = login.LoginUsuario;
            string pass = login.LoginPasswords;

            //RegistroUsuario modelRegUsuario = await _context.RegistroUsuarios.FirstOrDefaultAsync(x => x.Usuario == user && x.Passwords == pass);
            var modelRegUsuario = await _context.RegistroUsuarios.FirstOrDefaultAsync(x => x.Usuario == user);

            if (modelRegUsuario == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Credenciales incorrectas",
                    result = ""
                });
            }

            // Extraer el salt y el hash almacenados
            var storedPasswordParts = modelRegUsuario.Passwords.Split(':');
            string salt = storedPasswordParts[0];
            string storedHash = storedPasswordParts[1];

            // Verificar la contraseña ingresada usando el hash y el salt
            bool isValid = HashHelper.Verify(pass, storedHash, salt);

            if (!isValid)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Credenciales incorrectas",
                    result = ""
                });
            }

            // Generar el JWT después de la verificación exitosa
            var jwtConfig = _config.GetSection("Jwt").Get<Model_Jwt>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwtConfig.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", modelRegUsuario.IdUsuarios),
                new Claim("usuario", modelRegUsuario.Usuario),
                //new Claim("cedula", modelRegUsuario.Cedula),
                new Claim("email", modelRegUsuario.Email),
                //new Claim("fechaCreacion", modelRegUsuario.FechaCreacion),
                new Claim("rol", modelRegUsuario.Rol),
                //new Claim("foto", modelRegUsuario.Foto),
                //new Claim("estado", modelRegUsuario.Estado),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                //expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signIn
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                success = true,
                message = "!!!Inicio de sesion exitoso!!!",
                result = tokenString,
            });
        }

        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { success = false, message = "Token is required" });
            }

            // Agregar el token a una lista negra (blacklist)
            TokenBlacklist.Add(token);

            return Ok(new { success = true, message = "Logged out successfully" });
        }

        //[HttpPost]
        //[Route("ForgotPassword")]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgot)
        //{
        //    var user = await _context.RegistroUsuarios.FirstOrDefaultAsync(x => x.Email == forgot.Email);
        //    if (user == null)
        //    {
        //        return NotFound(new { success = false, message = "Usuario no encontrado" });
        //    }

        //    var token = Guid.NewGuid().ToString();
        //    var expiration = DateTime.UtcNow.AddDays(1); // El token expira en 1 hora

        //    var resetToken = new PasswordResetToken
        //    {
        //        Token = token,
        //        IdUsuarios = user.IdUsuarios,
        //        Expiration = expiration
        //    };

        //    _context.PasswordResetTokens.Add(resetToken);
        //    await _context.SaveChangesAsync();

        //    // Enviar correo electrónico con el token
        //    await _emailSender.SendPasswordResetEmailAsync(user.Email, token);

        //    return Ok(new { success = true, message = "Correo de recuperación enviado." });
        //}

        //[HttpPost]
        //[Route("ResetPassword")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resert)
        //{
        //    var resetToken = await _context.PasswordResetTokens.FirstOrDefaultAsync(x => x.Token == resert.Token && x.Expiration > DateTime.UtcNow);
        //    if (resetToken == null)
        //    {
        //        return NotFound(new { success = false, message = "Token no válido o expirado" });
        //    }

        //    var user = await _context.RegistroUsuarios.FirstOrDefaultAsync(x => x.IdUsuarios == resetToken.IdUsuarios);
        //    if (user == null)
        //    {
        //        return NotFound(new { success = false, message = "Usuario no encontrado" });
        //    }

        //    var salt = SaltHelper.GenerateSalt();
        //    var hashedPassword = HashHelper.Hash(resert.NewPassword, salt);
        //    user.Passwords = $"{salt}:{hashedPassword}";

        //    // Eliminar el token de recuperación usado
        //    _context.PasswordResetTokens.Remove(resetToken);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { success = true, message = "Contraseña actualizada con éxito." });
        //}
    }
}
