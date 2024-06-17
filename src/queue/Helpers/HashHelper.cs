using System.Security.Cryptography;
using System.Text;

namespace Sontiq.Queue.Helpers
{
    public static class HashHelper
    {
        static byte[] Hash(string value)
        {
            using var alg = SHA256.Create();
            return alg.ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        public static string ToHashString(string value)
        {
            var builder = new StringBuilder();
            foreach (byte b in Hash(value))
                builder.Append(b.ToString("X2"));
            return builder.ToString();
        }
    }
}
