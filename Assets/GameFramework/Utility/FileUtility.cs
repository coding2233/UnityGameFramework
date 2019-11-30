using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Wanderer.GameFramework
{
    public class FileUtility
    {
        /// <summary>
        /// 获取文件字节数组
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string path)
        {
            byte[] bs = File.ReadAllBytes(path);
            return bs;
        }

        /// <summary>
        /// 获取文件的md5值
        /// </summary>
        /// <param name="data">文件的数据</param>
        /// <returns></returns>
        public static string GetFileMD5(byte[] data)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            //计算字节数组的哈希值  
            byte[] toData = md5.ComputeHash(data);
            string fileMD5 = BitConverter.ToString(toData).Replace("-", "").ToLower();
            return fileMD5;
        }


    }

}