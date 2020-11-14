using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Wanderer.GameFramework
{
    public class FileUtility
    {  
        //加密密钥
        public const byte ENCRYPYKEY = 121;

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


    /// <summary>
    /// 先做个^的简单加密
    /// </summary>
    public class EncryptFileStream : FileStream
    {

        public EncryptFileStream(string path, FileMode mode) : base(path, mode)
        {
        }
        public EncryptFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
        {
        }

        public override int Read(byte[] array, int offset, int count)
        {
            int index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= FileUtility.ENCRYPYKEY;
            }
            return index;
        }



        public override void Write(byte[] array, int offset, int count)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= FileUtility.ENCRYPYKEY;
            }
            base.Write(array, offset, count);
        }


    }

    public class EncryptMemoryStream : MemoryStream
    {
        public EncryptMemoryStream(byte[] buffer):base(buffer)
        {

        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int index = base.Read(buffer, offset, count);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= FileUtility.ENCRYPYKEY;
            }
            return index;
           // return base.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= FileUtility.ENCRYPYKEY;
            }
            base.Write(buffer, offset, count);
        }

    }





}