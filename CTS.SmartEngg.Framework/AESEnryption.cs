// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CTS.SmartEngg.Framework
{
    public class AESEncryption
    {

        /// <summary>
        /// Method to Encrypt String As Bytes
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public byte[] EncryptStringAsBytes(string plainText, byte[] Key)
        {
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = null;
            StreamWriter swEncrypt = null;

            Aes aesAlg = null;
            byte[] IV = new byte[16];

            try
            {
                aesAlg = Aes.Create();
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = Key;
                aesAlg.GenerateIV();
                aesAlg.IV.CopyTo(IV, 0);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                swEncrypt = new StreamWriter(csEncrypt);
                swEncrypt.Write(plainText);
            }
            finally
            {
                if (swEncrypt != null)
                {
                    swEncrypt.Close();
                    swEncrypt.Dispose();
                }
                if (csEncrypt != null)
                {
                    csEncrypt.Close();
                    csEncrypt.Dispose();
                }
                if (msEncrypt != null)
                {
                    msEncrypt.Close();
                    msEncrypt.Dispose();
                }
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                    aesAlg.Dispose();
                }
            }

            var encArray = msEncrypt.ToArray();
            byte[] encStream = new byte[encArray.Length + 16];
            Array.Copy(encArray, encStream, encArray.Length);
            Array.Copy(IV, 0, encStream, encArray.Length, 16);
            return encStream;
        }

        /// <summary>
        /// Method to Decrypt String From Bytes
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string DecryptStringFromBytes(byte[] cipherText, byte[] Key)
        {
            if (cipherText == null || cipherText.Length < 32)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;

            Aes aesAlg = null;

            string plaintext = null;

            try
            {
                aesAlg = Aes.Create();
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = Key;
                byte[] IV = new byte[16];
                Array.Copy(cipherText, cipherText.Length - 16, IV, 0, 16);
                aesAlg.IV = GetIV(IV);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                msDecrypt = new MemoryStream(cipherText, 0, cipherText.Length - 16);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                plaintext = srDecrypt.ReadToEnd();
            }
            finally
            {
                if (srDecrypt != null)
                {
                    srDecrypt.Close();
                }
                if (csDecrypt != null)
                {
                    csDecrypt.Close();
                }
                if (msDecrypt != null)
                {
                    msDecrypt.Close();
                }
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            return plaintext;

        }


        /// <summary>
        /// Method to Decrypt the TicketDescription
        /// </summary>
        /// <param name="description">description</param>
        /// <param name="Key">Key</param>
        /// <returns>Decrypted String</returns>
        public string DecryptStringBytes(string description, byte[] Key)
        {
            string decodedText = null;
            try
            {
                decodedText = DecryptStringFromBytes(Convert.FromBase64String(description), Key);
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
                decodedText = string.Empty;
            }
            return HttpUtility.HtmlDecode(decodedText);
        }

        /// <summary>
        /// Get IV
        /// </summary>
        /// <param name="iV"></param>
        /// <returns></returns>
        private byte[] GetIV(byte[] iV)
        {
            return iV;
        }
    }
}
