using System;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;

namespace AwtterSDK.Editor
{
    internal static class TokenCache
    {
        static string _token;
        internal static string Token
        {
            get
            {
                if (_token == null)
                    ReadToken();

                return _token;
            }
            set
            {
                if (_token == value) return;

                _token = value;
                SaveToken(value);
            }
        }

        static void SaveToken(string rawToken)
        {
            if (string.IsNullOrEmpty(rawToken))
            {
                EditorPrefs.SetString("AwSdkKey", string.Empty);
                EditorPrefs.SetString("AwSdkToken", string.Empty);
                return;
            }

            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.KeySize = 128;
            crypto.BlockSize = 128;
            crypto.GenerateKey();

            EditorPrefs.SetString("AwSdkKey", Convert.ToBase64String(crypto.Key));
            EditorPrefs.SetString("AwSdkToken", AesOperation.EncryptString(crypto.Key, rawToken));
        }

        static void ReadToken()
        {
            string key = EditorPrefs.GetString("AwSdkKey");
            string token = EditorPrefs.GetString("AwSdkToken");

            if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(token))
                _token = AesOperation.DecryptString(Convert.FromBase64String(key), token);
        }
    }

    static class AesOperation
    {
        public static string EncryptString(byte[] key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
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

        public static string DecryptString(byte[] key, string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            byte[] iv = new byte[16];
            byte[] buffer = null;
            try
            {
                buffer = Convert.FromBase64String(cipherText);
            }
            catch (FormatException)
            {
                return string.Empty;
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
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
