using System.Security.Cryptography;
using System.Text;

namespace TaskTracker
{
    public static class Functions
    {
        static Functions()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public static string HashMD5(string text, string salt = "") 
        {
            string hashed = "";
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(text+salt);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                hashed = Convert.ToHexString(hashBytes);
            }

            return hashed;
        }

        public static string Encrypt(string text, string stringKey, string stringIv)
        {
            AesCryptoServiceProvider aesCryptoService = new AesCryptoServiceProvider();
            aesCryptoService.KeySize = 256;
            aesCryptoService.Padding = PaddingMode.PKCS7;
            byte[] iv = Encoding.GetEncoding(1250).GetBytes(stringIv); //16
            byte[] key = Encoding.GetEncoding(1250).GetBytes(stringKey); //32
            aesCryptoService.IV = iv;
            aesCryptoService.Key = key;
            ICryptoTransform transform = aesCryptoService.CreateEncryptor(key, iv);

            byte[] encrypted = Encoding.GetEncoding(1250).GetBytes(text);
            for (int i = 0; i < EncryptingIterations; i++)
                encrypted = transform.TransformFinalBlock(encrypted, 0, encrypted.Length);
            text = Convert.ToBase64String(encrypted, Base64FormattingOptions.InsertLineBreaks).Replace('+', '*');

            return text;
        }

        public static string Decrypt(string text, string stringKey, string stringIv)
        {
            try
            {
                AesCryptoServiceProvider aesCryptoService = new AesCryptoServiceProvider();
                aesCryptoService.KeySize = 256;
                aesCryptoService.Padding = PaddingMode.PKCS7;
                byte[] iv = Encoding.GetEncoding(1250).GetBytes(stringIv); //16
                byte[] key = Encoding.GetEncoding(1250).GetBytes(stringKey); ; //32
                aesCryptoService.IV = iv;
                aesCryptoService.Key = key;
                ICryptoTransform transform = aesCryptoService.CreateDecryptor(key, iv);

                byte[] decrypted = Convert.FromBase64String(text.Replace('*', '+'));
                for (int i = 0; i < EncryptingIterations; i++)
                    decrypted = transform.TransformFinalBlock(decrypted, 0, decrypted.Length);
                text = Encoding.GetEncoding(1250).GetString(decrypted);
            }
            catch { }

            return text;
        }
    }
}
