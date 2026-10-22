using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApiForm.Extension
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RequireExpirationTime = false,
                        ValidIssuer = config["Jwt:Issuer"], // Definir el emisor del token
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])) // Utiliza la clave secreta obtenida del entorno
                    };

                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var h = context.Request.Headers.Authorization.ToString();
                            Console.WriteLine($"[AUTH] largo={h.Length}, puntos={h.Count(c => c == '.')}, inicio={h[..Math.Min(40, h.Length)]}, fin={h[Math.Max(0, h.Length - 15)..]}");
                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"AUTH FAILED: {context.Exception.GetType().Name} - {context.Exception.Message}");
                            return Task.CompletedTask;
                        },

                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            string message;
                            if (string.IsNullOrEmpty(context.Request.Headers.Authorization))
                                message = "No se envió el token de autenticación.";
                            else if (context.AuthenticateFailure is SecurityTokenExpiredException)
                                message = "El token ha expirado.";
                            else
                                message = "El token no es válido.";

                            // TEMPORAL: solo para depurar, quítalo después
                            var detalle = context.AuthenticateFailure?.Message ?? context.ErrorDescription ?? "";

                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsJsonAsync(new { success = false, message, result = detalle });
                        }
                    };
                });

            return services;
        } 
    }
}
