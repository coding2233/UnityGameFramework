//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #网页请求 继承MonoBehaviour的实现类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月12日 11点16分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Taurus
{
    public sealed class WebRquestMonoHelper : MonoBehaviour,IWebRequestHelper
    {

        public void ReadHttpText(string url, Action<string, bool,string> result)
        {
            StartCoroutine(WWWReadHttpText(url, result));
        }
        

        IEnumerator WWWReadHttpText(string url, Action<string, bool,string> result)
        {
            WWW w = new WWW(url);
            yield return w;
            if (w.error != null)
                result.Invoke(url, false,w.error.ToString());
            else
                result.Invoke(url, true, w.text);
        }

    }
}
