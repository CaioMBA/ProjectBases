using System.Security.Cryptography;
using System.Text;

namespace Domain.Extensions
{
    public static class CryptographyExtension
    {
        private static readonly string _encryptionKey = "F0=jobOfI@HLy1yayiyepRrpRLspamEh";
        private static readonly string _secretKeyTo256 = "zXv5D3G9pR8fs4VX1JHD8zVlPzIa1Ug0CQ3Rk8MDSUQ=";

        public static string GenerateHash(String plainText)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(_secretKeyTo256)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                return Convert.ToBase64String(hash);
            }
        }
        public static string Encrypt(this String plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(iv, 0, iv.Length); // Armazena o IV no início do stream
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
        public static string Decrypt(this String encryptedText)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);

                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] plainBytes = memoryStream.ToArray();
                        return Encoding.UTF8.GetString(plainBytes);
                    }
                }
            }
        }
    }
}
