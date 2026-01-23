using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using WebApiForm.Capa_de_Servicio;
using WebApiForm.DTO__Data_Transfer_Object_;

namespace WebApiForm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordRecoveryController : ControllerBase
    {
        private readonly PasswordRecoveryService _passService;

        public PasswordRecoveryController(PasswordRecoveryService passService)
        {
            _passService = passService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPasswordRecovery([FromBody] ForgotPasswordDto forgotEmail)
        {
            if (forgotEmail == null || string.IsNullOrEmpty(forgotEmail.Email))
            {
                return BadRequest(new { status = "error", message = "El cuerpo de la solicitud debe contener el correo electrónico." });
            }

            try
            {
                var request = await _passService.CrearTokenRecuperacionAsync(forgotEmail.Email);
                if (request)
                {
                    return Ok(new { status = "success", message = "Correo de recuperación enviado." });

                }
                else
                {
                    return BadRequest(new { status = "error", message = "Error al enviar el correo de recuperación." });
                }

                //return Ok("Correo de recuperación enviado.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = "Error general al enviar la solicitud de recuperación de contraseña.", details = ex.Message });
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest(new { message = "El cuerpo de la solicitud debe contener el token y la nueva contraseña." });
            }

            try
            {
                var reset = await _passService.VerificarTokenAsync(model.Token, model.NewPassword);
                if (reset)
                {
                    return Ok(new { status = "success", message = "Contraseña restablecida con éxito." });
                }
                else
                {
                    return BadRequest(new { status = "error", message = "Token inválido o expirado." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error general al restablecer la contraseña", details = ex.Message });
            }
        }
    }
}
