using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Jerro.Settings.Encryption
{
    /// <summary>
    /// Default Data Protector
    /// </summary>
    public class DefaultDataProtector : IDataProtector
    {
        private readonly string _Key = "JerroSettings2022";

        /// <summary>
        /// Initializes an instance of <see cref="DefaultDataProtector"/>.
        /// </summary>
        public DefaultDataProtector()
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="DefaultDataProtector"/>.
        /// </summary>
        public DefaultDataProtector(string key)
        {
            _Key = key;
        }

        string IDataProtector.Protect(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_Key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        string IDataProtector.Unprotect(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_Key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}