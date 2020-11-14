using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public static class StringExtensions
    {
        public static bool ToBool(this string value)
        {
            bool result = false;
            bool.TryParse(value.ToLower(), out result);
            return result;
        }

        public static int ToInt32(this string value)
        {
            int result = 0;
            int.TryParse(value, out result);
            return result;
        }

        public static long ToInt64(this string value)
        {
            long result = 0;
            long.TryParse(value, out result);
            return result;
        }

        public static float ToFloat(this string value)
        {
            float result = 0;
            float.TryParse(value, out result);
            return result;
        }

        public static double ToDouble(this string value)
        {
            double result = 0;
            double.TryParse(value, out result);
            return result;
        }

        public static Vector2 ToVector2(this string value)
        {
            Vector2 result = Vector2.zero;
            string[] args = value.Split(',');
            if (args.Length == 2)
            {
                result.x = args[0].ToFloat();
                result.y = args[1].ToFloat();
            }
            return result;
        }

        public static Vector3 ToVector3(this string value)
        {
            Vector3 result = Vector3.zero;
            string[] args = value.Split(',');
            if (args.Length == 3)
            {
                result.x = args[0].ToFloat();
                result.y = args[1].ToFloat();
                result.z = args[2].ToFloat();
            }
            return result;
        }

        public static Color ToColor(this string value)
        {
            Color result = Color.white;
            UnityEngine.ColorUtility.TryParseHtmlString(value.Trim(), out result);
            return result;
        }



        public static string ToByteLengthString(this long size)
        {
            if (size < 1024)
            {
                return $"{size}B";
            }
            else if (size < 1024 * 1024)
            {
                return $"{size / 1024}KB";
            }
            else
            {
                return $"{(size / 1024.0f / 1024.0f).ToString("f2")}MB";
            }
        }

        public static string ToByteLengthString(this int size)
        {
            if (size < 1024)
            {
                return $"{size}B";
            }
            else if (size < 1024 * 1024)
            {
                return $"{size / 1024}KB";
            }
            else
            {
                return $"{(size / 1024.0f / 1024.0f).ToString("f2")}MB";
            }
        }

        public static string ToByteLengthString(this uint size)
        {
            if (size < 1024)
            {
                return $"{size}B";
            }
            else if (size < 1024 * 1024)
            {
                return $"{size / 1024}KB";
            }
            else
            {
                return $"{(size / 1024.0f / 1024.0f).ToString("f2")}MB";
            }
        }


        /// <summary>
        /// 字符串异或
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToEncrypt(this string str)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= FileUtility.ENCRYPYKEY;
            }
            str = System.Text.Encoding.UTF8.GetString(data);
            return str;
        }

        /// <summary>
        /// 获取字符串的MD5
        /// </summary>
        /// <param name="data">文件的数据</param>
        /// <returns></returns>
        public static string GetMD5(this string str)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(str);
            //计算字节数组的哈希值  
            byte[] toData = md5.ComputeHash(data);
            string fileMD5 = BitConverter.ToString(toData).Replace("-", "").ToLower();
            return fileMD5;
        }

    }
}

