//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Hu Tao. All rights reserved.
// </copyright>
// <describe> #加密器# </describe>
// <email> 987947865@qq.com </email>
// <time> #2018年6月28日 11点16分# </time>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GameFramework.Taurus
{
    public static class Encipherer
    {
        /// <summary>
        /// AES 加密
        /// </summary>
        public static string AesEncrypt(string str, EnciphererKey key)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key.Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// AES 解密
        /// </summary>
        public static string AesDecrypt(string str, EnciphererKey key)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            byte[] toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key.Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// AES 加密
        /// </summary>
        public static byte[] AESEncrypt(byte[] array, EnciphererKey key)
        {
            if (array.Length <= 0)
                return null;

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key.Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            if (key.KeyVector.Length == 0)
            {
                rm.GenerateIV();
                key.KeyVector = rm.IV;
            }
            else
            {
                rm.IV = key.KeyVector;
            }

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, rm.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(array, 0, array.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();
            cs.Close();
            ms.Close();
            return cipherBytes;
        }
        /// <summary>
        /// AES 解密
        /// </summary>
        public static byte[] AESDecrypt(byte[] array, EnciphererKey key)
        {
            if (array.Length <= 0)
                return null;

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key.Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            if (key.KeyVector.Length == 0)
            {
                rm.GenerateIV();
                key.KeyVector = rm.IV;
            }
            else
            {
                rm.IV = key.KeyVector;
            }

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, rm.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(array, 0, array.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();
            cs.Close();
            ms.Close();
            return cipherBytes;
        }
    }
}
