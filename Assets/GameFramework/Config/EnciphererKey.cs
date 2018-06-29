//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Hu Tao. All rights reserved.
// </copyright>
// <describe> #加密器密钥序列化对象# </describe>
// <email> 987947865@qq.com </email>
// <time> #2018年6月28日 14点53分# </time>
//-----------------------------------------------------------------------


using UnityEngine;

public class EnciphererKey : ScriptableObject {

    public string Key = "";
    public byte[] KeyVector;
    
    public void GeneraterKey()
    {
        string tem = "0123456789abcdefghijklmnopqrstuvwxyz";
        for (int i = 0; i < 32; i++)
        {
            int index = Random.Range(0, tem.Length);
            Key += tem[index];
        }
        KeyVector = new byte[0];
    }
}
