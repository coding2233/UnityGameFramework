using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace Wanderer.GameFramework
{
    public class SetUTF8WithoutBOM
    {
        [MenuItem("Assets/Set the text to UTF-8 without BOM encoding")]
        private static void SetTextEncoding2UTF8withoutBOM()
        {
            if (Selection.objects == null)
                return;


            if (Selection.assetGUIDs.Length == 1)
            {
                string selectPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
                if (AssetDatabase.IsValidFolder(selectPath))
                {
                    var assets = AssetDatabase.FindAssets("t:TextAsset", new string[] { selectPath });
                    if (assets != null)
                    {
                        bool reEncoding = false;
                        foreach (var item in assets)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(item);
                            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                            if (EncodingType.GetType(assetPath) == Encoding.UTF8)
                            {
                                if (EncodingType.HasBom(asset.bytes))
                                {
                                    byte[] newData = new byte[asset.bytes.Length - 3];
                                    Array.Copy(asset.bytes, 3, newData, 0, newData.Length);
                                    File.WriteAllBytes(assetPath, newData);
                                    Debug.Log($"UTF-8 重新编码,设置为Without BOM: {assetPath}");
                                    reEncoding = true;
                                }
                            }
                            else
                            {
                                Debug.Log($"非UTF-8编码,请检查编码格式(暂不做自动处理) : {assetPath}");
                            }
                        }
                        if (reEncoding)
                        {
                            AssetDatabase.Refresh();
                        }
                    }

                }
            }
        }
    }



        /// <summary> 
        /// 获取文件的编码格式 
        /// </summary> 
        public class EncodingType
        {
            /// <summary> 
            /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
            /// </summary> 
            /// <param name=“FILE_NAME“>文件路径</param> 
            /// <returns>文件的编码类型</returns> 
            public static System.Text.Encoding GetType(string FILE_NAME)
            {
                FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
                Encoding r = GetType(fs);
                fs.Close();
                return r;
            }

            /// <summary> 
            /// 通过给定的文件流，判断文件的编码类型 
            /// </summary> 
            /// <param name=“fs“>文件流</param> 
            /// <returns>文件的编码类型</returns> 
            public static System.Text.Encoding GetType(FileStream fs)
            {
                byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
                byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
                byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
                Encoding reVal = Encoding.Default;

                BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
                int i;
                int.TryParse(fs.Length.ToString(), out i);
                byte[] ss = r.ReadBytes(i);
                if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
                {
                    reVal = Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    reVal = Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    reVal = Encoding.Unicode;
                }
                r.Close();
                return reVal;

            }

            /// <summary> 
            /// 判断是否是不带 BOM 的 UTF8 格式 
            /// </summary> 
            /// <param name=“data“></param> 
            /// <returns></returns> 
            public static bool IsUTF8Bytes(byte[] data)
            {
                int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
                byte curByte; //当前分析的字节. 
                for (int i = 0; i < data.Length; i++)
                {
                    curByte = data[i];
                    if (charByteCounter == 1)
                    {
                        if (curByte >= 0x80)
                        {
                            //判断当前 
                            while (((curByte <<= 1) & 0x80) != 0)
                            {
                                charByteCounter++;
                            }
                            //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                            if (charByteCounter == 1 || charByteCounter > 6)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //若是UTF-8 此时第一位必须为1 
                        if ((curByte & 0xC0) != 0x80)
                        {
                            return false;
                        }
                        charByteCounter--;
                    }
                }
                if (charByteCounter > 1)
                {
                    throw new Exception("非预期的byte格式");
                }
                return true;
            }


            public static bool HasBom(byte[] data)
            {
                bool hasBom = (data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF);
                return hasBom;   
            }
        }
 }
