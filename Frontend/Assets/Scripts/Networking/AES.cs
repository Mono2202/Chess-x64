using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class AES
{
    // Fields:
    public AesManaged aes;

    /*
     * C'tor
     */
    public AES()
    {
        // Inits:
        aes = new AesManaged();

        // Setting the Key and the IV:
        aes.Key = new byte[32]
            {
            0x37, 0x69, 0xC6, 0x4E, 0x64, 0x95, 0x83, 0xE2, 0x56, 0x6B, 0xB2, 0x3F,
            0x25, 0x35, 0xC3, 0xB2, 0xBE, 0x83, 0xF0, 0x83, 0x39, 0x49, 0xF5, 0x6E,
            0xC6, 0x00, 0x75, 0x67, 0x70, 0x99, 0xAD, 0x38
            };

        aes.IV = new byte[16]
        {
            0x04, 0xF1, 0x8B, 0xA0, 0xAF, 0x2F, 0x0B, 0x3A, 0xE9, 0x7C, 0x90, 0x6D,
            0xDF, 0x0D, 0x6B, 0x84
        };
    }

    /*
     * Encrypting with AES
     * Input : data - the data to encrypt
     * Output: the encrypted data
     */
    public string AESEncrypt(string data)
    {
        // Converting " to ^:
        string newData = "";
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == '\"')
            {
                newData += '^';
            }

            else
            {
                newData += data[i];
            }
        }

        // Inits:
        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        byte[] encrypted;

        // Encrypting with AES:
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(newData);
                }

                encrypted = ms.ToArray();
            }
        }

        // Converting text data to binary:
        encrypted = Encoding.ASCII.GetBytes(BitConverter.ToString(encrypted));
        return string.Join("", encrypted.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
    }

    /*
     * Decrypting with AES
     * Input : cipher  - the data to decrypt
     * Output: newData - the decrypted data
     */
    public string AESDecrypt(string cipher)
    {
        // Converting binary data to text:
        cipher = cipher.Replace("\0", String.Empty);
        var list = new List<Byte>();
        for (int i = 0; i < cipher.Length; i += 8)
        {
            string t = cipher.Substring(i, 8);
            list.Add(Convert.ToByte(t, 2));
        }
        cipher = Encoding.ASCII.GetString(list.ToArray());

        // Converting hex string to byte array:
        string[] hexValuesSplit = cipher.Split('-');
        byte[] byteValues = new byte[hexValuesSplit.Length];
        for (int i = 0; i < hexValuesSplit.Length; i++)
        {
            // Condition: last hex value:
            if (i == hexValuesSplit.Length - 1)
            {
                hexValuesSplit[i] = hexValuesSplit[i].Split('\n')[0];
            }

            int value = Convert.ToInt32(hexValuesSplit[i], 16);
            byteValues[i] = (byte)value;
        }

        // Inits:
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        string data = "";

        // Decrypting with AES:    
        using (MemoryStream ms = new MemoryStream(byteValues))
        {
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader reader = new StreamReader(cs))
                {
                    data = reader.ReadToEnd();
                }
            }
        }

        // Converting ^ to ":
        string newData = "";
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == '^')
            {
                newData += '\"';
            }

            else
            {
                newData += data[i];
            }
        }

        return newData;
    }
}
