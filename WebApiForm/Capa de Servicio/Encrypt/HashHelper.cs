using System.Security.Cryptography;
using System.Text;

namespace WebApiForm.Capa_de_Servicio.Encrypt
{
    public class HashHelper
    {
        private const int HashSize = 20; // Tamaño del hash en bytes
        private const int Iterations = 10000; // Número de iteraciones para el algoritmo

        public static string Hash(string pass, string salt)
        {
            // Convertir el salt de Base64 a byte array
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Generar el hash
            var pbkdf2 = new Rfc2898DeriveBytes(pass, saltBytes, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Combinar salt y hash
            byte[] hashBytes = new byte[saltBytes.Length + HashSize];
            Array.Copy(saltBytes, 0, hashBytes, 0, saltBytes.Length);
            Array.Copy(hash, 0, hashBytes, saltBytes.Length, HashSize);

            // Convertir a cadena base64
            return Convert.ToBase64String(hashBytes);
        }

        public static bool Verify(string enteredPass, string storedHash, string salt)
        {
            // Convertir el salt de Base64 a byte array
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Obtener bytes de la cadena base64
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Hash de la contraseña introducida
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPass, saltBytes, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Comparar hash
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + saltBytes.Length] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
