//https://cymbeline.ch/2014/02/28/dynamic-aes-key-exchange-through-rsa-encryption/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Toems_Service
{
    public class ServiceSymmetricEncryption
    {
        public byte[] EncryptData(byte[] key, string data)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] result;

                aes.Key = key;
                aes.GenerateIV();

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(cs))
                            {
                                writer.Write(data);
                            }
                        }

                        byte[] encrypted = ms.ToArray();
                        result = new byte[aes.BlockSize / 8 + encrypted.Length];

                        // Result is built as: IV (plain text) + Encrypted(data)
                        Array.Copy(aes.IV, result, aes.BlockSize / 8);
                        Array.Copy(encrypted, 0, result, aes.BlockSize / 8, encrypted.Length);

                        return result;
                    }
                }
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<string> DecryptAsync(byte[] key, byte[] data)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Extract the IV from the data first.
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(data, iv, iv.Length);
                aes.IV = iv;

                // The remainder of the data is the encrypted data we care about.
                byte[] encryptedData = new byte[data.Length - iv.Length];
                Array.Copy(data, iv.Length, encryptedData, 0, encryptedData.Length);

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    using (MemoryStream ms = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cs))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
        public string Decrypt(byte[] key, byte[] data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Extract the IV from the data first.
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(data, iv, iv.Length);
                aes.IV = iv;

                // The remainder of the data is the encrypted data we care about.
                byte[] encryptedData = new byte[data.Length - iv.Length];
                Array.Copy(data, iv.Length, encryptedData, 0, encryptedData.Length);

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    using (MemoryStream ms = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cs))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
