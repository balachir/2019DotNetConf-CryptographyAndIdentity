using System;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp_SymmetricCryptography
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new SymmetricCryptographyDemo();
            s.PlainText = "Hello how are you";

            Console.WriteLine("Plain Text: {0}", s.PlainText);

            Console.WriteLine("Encrypting Data ...");
            s.EncryptData();
            Console.WriteLine("IV: {0}", s.IV);
            Console.WriteLine("Cipher Text: {0}", s.CipherText);

            Console.WriteLine("Decrypting Data ...");
            s.DecryptData();
            Console.WriteLine("Original Plain Text: {0}", s.OriginalPlainText);
        }
    }

    public class SymmetricCryptographyDemo
    {
        public string PlainText { get; set; }
        public string CipherText { get; set; }
        public string IV { get; set; }
        public string OriginalPlainText { get; set; }

        private Aes CreateCipher()
        {
            Aes cipher = Aes.Create();  //Defaults - Keysize 256, Mode CBC, Padding PKC27
            //Aes cipher = new AesManaged();
            //Aes cipher = new AesCryptoServiceProvider();

            cipher.Padding = PaddingMode.ISO10126;

            //cipher.Padding = PaddingMode.Zeros;
            //cipher.Mode = CipherMode.ECB;

            //Create() makes new key each time, use a consistent key for encrypt/decrypt
            //Key is a string value for demo purpose only. Normally, you shouldn't save the key directly in the code like below
            cipher.Key = conversions.HexToByteArray("892C8E496E1E33355E858B327BC238A939B7601E96159F9E9CAD0519BA5055CD"); ;

            return cipher;
        }

        public void EncryptData()
        {
            Aes cipher = CreateCipher();

            //Show the IV on page (will use for decrypt, normally send in clear along with ciphertext)
            IV = Convert.ToBase64String(cipher.IV);

            //Create the encryptor, convert to bytes, and encrypt
            ICryptoTransform cryptTransform = cipher.CreateEncryptor();
            byte[] plaintext = Encoding.UTF8.GetBytes(PlainText);
            byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

            //Convert to base64 for display
            CipherText = Convert.ToBase64String(cipherText);

        }

        public void DecryptData()
        {
            Aes cipher = CreateCipher();

            //Read back in the IV used to randomize the first block
            cipher.IV = Convert.FromBase64String(IV);

            //Create the decryptor, convert from base64 to bytes, decrypt
            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] cipherText = Convert.FromBase64String(CipherText);
            byte[] plainText = cryptTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);

            OriginalPlainText = Encoding.UTF8.GetString(plainText);
        }

    }

    public static class conversions
    {
        public static byte[] HexToByteArray(string hexString)
        {
            if (0 != (hexString.Length % 2))
            {
                throw new ApplicationException("Hex string must be multiple of 2 in length");
            }

            int byteCount = hexString.Length / 2;
            byte[] byteValues = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
            {
                byteValues[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return byteValues;
        }

        public static string ByteArrayToHex(byte[] data)
        {
            //This converts the 64 byte hash into the string hex representation of byte values 
            // (shown by default as 2 hex characters per byte) that looks like 
            // "FB-2F-85-C8-85-67-F3-C8-CE-9B-79-9C-7C-54-64-2D-0C-7B-41-F6...", each pair represents
            // the byte value of 0-255.  Removing the "-" separator creates a 128 character string 
            // representation in hex
            return BitConverter.ToString(data).Replace("-", "");
        }
    }
}
