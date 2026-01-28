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
    public class SmtpEmailSender : IEmailSender, IDisposable
    {
        private string _cachedToken;
        private DateTime _tokenExpiration;
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _tenantId;
        private readonly string _clientSecret;
        private readonly string _senderEmail;
        private readonly string _tokenUrl;
        private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);

        public SmtpEmailSender()
        {
            // Leer y validar variables de entorno UNA SOLA VEZ
            _clientId = Environment.GetEnvironmentVariable("ClientId");
            _tenantId = Environment.GetEnvironmentVariable("TenantId");
            _clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
            _senderEmail = Environment.GetEnvironmentVariable("SenderEmail");

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_tenantId) ||
                string.IsNullOrEmpty(_clientSecret) || string.IsNullOrEmpty(_senderEmail))
            {
                throw new Exception("Faltan variables de entorno: ClientId, ClientSecret, TenantId y SenderEmail");
            }

            _tokenUrl = $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/token";

            // HttpClient singleton con configuración optimizada para Graph API
            _httpClient = new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                MaxConnectionsPerServer = 10
            })
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        private async Task<string> GetAccessTokenAsync()
        {
            // Verificación rápida sin lock
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiration)
            {
                return _cachedToken;
            }

            // Lock solo cuando necesitamos renovar el token
            await _tokenLock.WaitAsync();
            try
            {
                // Double-check después del lock (por si otro thread ya renovó)
                if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiration)
                {
                    return _cachedToken;
                }

                var body = new Dictionary<string, string>
            {
                {"client_id", _clientId},
                {"scope", "https://graph.microsoft.com/.default"},
                {"client_secret", _clientSecret},
                {"grant_type", "client_credentials"}
            };

                var response = await _httpClient.PostAsync(_tokenUrl, new FormUrlEncodedContent(body));

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al obtener token: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(json);

                _cachedToken = result.AccessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 60);

                return _cachedToken;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        public async Task SendEmail(string toEmail, string subject, string htmlContent)
        {
            var token = await GetAccessTokenAsync();

            var mail = new EmailRequest
            {
                Message = new Message
                {
                    Subject = subject,
                    Body = new Body { ContentType = "HTML", Content = htmlContent },
                    ToRecipients = new[]
                    {
                    new Recipient { EmailAddress = new EmailAddress { Address = toEmail } }
                }
                },
                SaveToSentItems = false
            };

            var json = JsonConvert.SerializeObject(mail);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post,
                $"https://graph.microsoft.com/v1.0/users/{_senderEmail}/sendMail")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error enviando correo: {error}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _tokenLock?.Dispose();
        }
    }

    // Clases para deserialización tipada (más rápida que dynamic)
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class EmailRequest
    {
        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("saveToSentItems")]
        public bool SaveToSentItems { get; set; }
    }

    public class Message
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("toRecipients")]
        public Recipient[] ToRecipients { get; set; }
    }

    public class Body
    {
        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class Recipient
    {
        [JsonProperty("emailAddress")]
        public EmailAddress EmailAddress { get; set; }
    }

    public class EmailAddress
    {
        [JsonProperty("address")]
        public string Address { get; set; }
    }
}