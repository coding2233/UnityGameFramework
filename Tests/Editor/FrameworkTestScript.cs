using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Wanderer.GameFramework
{
    public class FrameworkTestScript
    {
        [Test]
        public void UtilityAESTestPasses()
        {
            string content = "测试zhang账号密码";
            string key = "thanks,.........";
            Log.Info($"加密信息: {content} 密钥: {key}");
            string data = Utility.AESEncrypt(content, key,System.Security.Cryptography.CipherMode.CBC);
            Log.Info($"加密后 {data}");
            string newdata = Utility.AESDecrypt(data, key, System.Security.Cryptography.CipherMode.CBC);
            Log.Info($"解密后 {newdata}");
        }

        //[Test]
        //public void CacheTestPasses()
        //{
        //    Log.Info($"缓存大小: {GameFrameworkMode.GetCacheSize()}");
        //}

        // A Test behaves as an ordinary method
        [Test]
        public void FrameworkTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FrameworkTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }

}