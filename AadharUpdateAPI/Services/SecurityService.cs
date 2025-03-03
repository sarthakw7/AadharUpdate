using System.Security.Cryptography;
using System.Text;

namespace AadharUpdateAPI.Services
{
    public interface ISecurityService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        string EncryptData(string data);
        string DecryptData(string encryptedData);
    }

    public class SecurityService : ISecurityService
    {
        private readonly string _encryptionKey;

        public SecurityService(IConfiguration configuration)
        {
            _encryptionKey = configuration["Security:EncryptionKey"] ?? "YourDefaultEncryptionKey12345";
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string EncryptData(string data)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(data);
                        }

                        array = memoryStream.ToArray();
                    }
                }

                return Convert.ToBase64String(array);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error encrypting data", ex);
            }
        }

        public string DecryptData(string encryptedData)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(encryptedData);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error decrypting data", ex);
            }
        }
    }
} 