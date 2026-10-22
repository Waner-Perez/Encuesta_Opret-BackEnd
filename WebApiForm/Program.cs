using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApiForm.Capa_de_Servicio;
using WebApiForm.Capa_de_Servicio.Exportacion;
using WebApiForm.Extension;
using WebApiForm.Interfaces;
using WebApiForm.Middleware;
using WebApiForm.Repository;

Env.Load(); // Lee y carga las variables en el entorno
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//var DefaultConnection = $"Server={Environment.GetEnvironmentVariable("PR_DATABASE_SERVER")}," +
//                        $"{Environment.GetEnvironmentVariable("PR_DATABASE_PORT")}; " +
//                        $"Database={Environment.GetEnvironmentVariable("PR_DATABASE_NAME")}; " +
//                        $"User Id={Environment.GetEnvironmentVariable("PR_DATABASE_USER")}; " +
//                        $"Password={Environment.GetEnvironmentVariable("PR_DATABASE_PASSWORD")}; " +
//                        $"Encrypt=True;" +
//                        $"TrustServerCertificate=True;";

var connectionTesting = config.GetConnectionString("DBConnection") ?? throw new InvalidOperationException("Connection string 'DBConnection' not found.");

// Add services to the container.
builder.Services.AddDbContext<FormEncuestaDbContext>(Options => Options.UseSqlServer(
    connectionTesting
    /*DefaultConnection*/
));

// Agrega el servicio de controladores
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiForm", Version = "1.0" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer Token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

//Configuracion del Token
builder.Services.AddAuthentication(config);

// Registrar el servicio en vace a stored procedure
builder.Services.AddScoped<FormularioRepository>();
//--------------------------------------------------------
builder.Services.AddScoped<EstacionPorLineaService>();
builder.Services.AddScoped<PreguntaCompletaService>();
builder.Services.AddScoped<RespuestaService>();
builder.Services.AddScoped<FormularioServices>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<PasswordRecoveryService>();
builder.Services.AddMemoryCache();
//--------------------------------------------------------

// Configura CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//para que se pueda visualizar en el hosting
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

// Usa CORS
app.UseCors("AllowAll");

// Usa autenticación
app.UseAuthentication();

// Usa el middleware de lista negra de tokens
app.UseTokenBlacklist(); // Asegúrate de que este llamado esté después de la autenticación

// Usa la autorización
app.UseAuthorization();

app.MapControllers();

app.Run();
