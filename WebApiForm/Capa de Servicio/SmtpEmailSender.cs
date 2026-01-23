using DotNetEnv;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using MimeKit;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApiForm.Interfaces;

namespace WebApiForm.Capa_de_Servicio
{
    public class SmtpEmailSender : IEmailSender
    {
        /*
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public SmtpEmailSender(IConfiguration configuration, IMemoryCache cache)
        {
            _cache = cache;
            _config = configuration;
        }*/

        /*
         
        public async Task SendEmail(string toEmail, string subject, string plainTextContent, string htmlContent)
        {
            //Leer configuracion SMTP
            var smtpConfig = _config.GetSection("Smtp");
            var host = smtpConfig["Host"];
            int port = 587; // Puerto para TLS
            string fromEmail = "reply_no@hotmail.com"; // Correo que aparecerá como remitente

            //Credenciales de Azure AD directamente desde variables de entorno
            var clientId = Environment.GetEnvironmentVariable("ClientId");
            var clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
            var tenantId = Environment.GetEnvironmentVariable("TenantId");

            //Solicitar token OAuth2(client credentials flow)
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .Build();

            var scopes = new[] { "https://graph.microsoft.com/.default" }; // Alcance para enviar correos
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync(); // Obtener el token de acceso
            var accessToken = result.AccessToken; // Token de acceso para autenticación OAuth2

            // Crear el mensaje de correo
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(fromEmail)); // Usar el correo de remitente configurado
            message.To.Add(new MailboxAddress("Cliente Usuario", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = plainTextContent,
                HtmlBody = htmlContent
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Enviar correo usando OAuth2
            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls); // Conectar usando TLS por el puerto 587
            var oauth2 = new SaslMechanismOAuth2(fromEmail, accessToken);
            await client.AuthenticateAsync(oauth2); // Autenticación con OAuth2
            await client.SendAsync(message); // Enviar el mensaje
            await client.DisconnectAsync(true); // Desconectar del servidor SMTP
        }
        */

        /* de Gmail
        public async Task SendEmail(string toEmail, string subject, string plainTextContent, string htmlContent)
        {
            // Cargar configuración SMTP desde appsettings.json
            //var smtpConfig = _config.GetSection("Smtp");
            string host = "smtp.gmail.com";
            int port = 587; //o sino por el puerto "465" para SSL
            //var username = smtpConfig["Username"];
            var username = Environment.GetEnvironmentVariable("Username"); //uso de variable de entorno
            //var password = smtpConfig["Password"];
            var password = Environment.GetEnvironmentVariable("Password");
            string fromEmail = "reply_no@gmail.com";

            // Crear el mensaje de correo
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Soporte Técnico", ""));
            //message.From.Add(MailboxAddress.Parse(fromEmail));
            message.To.Add(new MailboxAddress("Cliente Usuario", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = plainTextContent,
                HtmlBody = htmlContent
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Enviar el correo
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls); //en caso de utilizar SSL debe de aplicar (SecureSocketOptions.SslOnConnect)
                    await smtpClient.AuthenticateAsync(username, password);
                    await smtpClient.SendAsync(message);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar correo: " + ex.Message);
                throw;
            }
        }*/


        //version con microsoft Graph API y cuenta organizacional

        private string _cachedToken;
        private DateTime _tokenExpiration;

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiration)
            {
                return _cachedToken;
            }

            //Credenciales de Azure AD directamente desde variables de entorno
            var clientId = Environment.GetEnvironmentVariable("ClientId");
            var tenantId = Environment.GetEnvironmentVariable("TenantId");
            var clientSecret = Environment.GetEnvironmentVariable("ClientSecret");

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("Faltan variables de entorno requiere ClientId, ClientSecret y TenantId");
            }

            var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            var body = new Dictionary<string, string>
            {
                {"client_id", clientId},
                {"scope", "https://graph.microsoft.com/.default"},
                {"client_secret", clientSecret},
                {"grant_type", "client_credentials"}
            };

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(body));
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al obtener token: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            _cachedToken = result.access_token;
            _tokenExpiration = DateTime.UtcNow.AddSeconds((int)result.expires_in - 60); // renovar 1 min antes

            return _cachedToken;
        }

        public async Task SendEmail(string toEmail, string subject, string htmlContent)
        {
            var senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
            var token = await GetAccessTokenAsync();

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var mail = new
            {
                message = new
                {
                    subject = subject,
                    body = new { contentType = "HTML", content = htmlContent },
                    toRecipients = new[]
                {
                    new { emailAddress = new { address = toEmail } }
                }
                },
                saveToSentItems = "false"
            };

            var json = JsonConvert.SerializeObject(mail);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"https://graph.microsoft.com/v1.0/users/{senderEmail}/sendMail",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error enviando correo: {error}");
            }
        }
    }
}