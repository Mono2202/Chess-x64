using System;
using System.IO;
using System.Security.Cryptography;
class Aes
{
    public static void Main(string[] args)
    {
        // Inits:
        AesManaged aes = new AesManaged();

        // Reading the Key and the IV:
        aes.Key = File.ReadAllBytes("KEY");
        aes.IV = File.ReadAllBytes("IV");

        // AES Encryption:
        if (args[0] == "e")
        {
            string enc = AESEncrypt(aes, args[1], aes.Key, aes.IV);
            Console.WriteLine(enc);
        }

        // AES Decryption:
        if (args[0] == "d")
        {
            // Converting hex string to byte array:
            string[] hexValuesSplit = args[1].Split('-');
            byte[] byteValues = new byte[hexValuesSplit.Length];
            for (int i = 0; i < hexValuesSplit.Length; i++)
            {
                int value = Convert.ToInt32(hexValuesSplit[i], 16);
                byteValues[i] = (byte)value;
            }

            // Decrypting the string:
            string dec = AESDecrypt(aes, byteValues, aes.Key, aes.IV);
            Console.WriteLine(dec);
        }
    }

    private static void WriteToBinaryFile(string fileName, byte[] data)
    {
        // Writing to a binary file:
        using var stream = File.Create(fileName);
        stream.Write(data, 0, data.Length);
    }

    private static string AESEncrypt(AesManaged aes, string data, byte[] key, byte[] iv)
    {
        // Inits:
        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
        byte[] encrypted;

        // Encrypting with AES:
        using (MemoryStream ms = new MemoryStream())
        {  
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {  
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(data);
                }

                encrypted = ms.ToArray();
            }
        }

        return BitConverter.ToString(encrypted);
    }

    private static string AESDecrypt(AesManaged aes, byte[] cipher, byte[] key, byte[] iv)
    {
        // Inits:
        ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
        string data = "";

        // Decrypting with AES:    
        using (MemoryStream ms = new MemoryStream(cipher))
        {    
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {   
                using (StreamReader reader = new StreamReader(cs))
                {
                    data = reader.ReadToEnd();
                }
            }
        }

        return data;
    }
}