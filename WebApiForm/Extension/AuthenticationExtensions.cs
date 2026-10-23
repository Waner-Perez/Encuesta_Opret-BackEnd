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
                        RequireExpirationTime = true,
                        ValidIssuer = config["Jwt:Issuer"], // Definir el emisor del token
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])) // Utiliza la clave secreta obtenida del entorno
                    };
                });

            return services;
        } 
    }
}
