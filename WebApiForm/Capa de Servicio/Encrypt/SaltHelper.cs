using System.Security.Cryptography;

namespace WebApiForm.Capa_de_Servicio.Encrypt
{
    public static class SaltHelper
    {
        public static string GenerateSalt(int size = 32)
        {
            var salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return Convert.ToBase64String(salt);
        }
    }
}
