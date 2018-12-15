//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #复制 hotfix dll# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 18点03分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class HotFixBuildEditor  {

	private const string _srcDllPath = "Library/ScriptAssemblies/HotFix.dll";
	//pdb...

	private const string _destDllPath = "Assets/Game/HotFix/HotFix.dll.bytes";


	[InitializeOnLoadMethod]
	static void Main()
	{
		//复制dll
		File.Copy(_srcDllPath, _destDllPath, true);
		
		//刷新资源
		AssetDatabase.Refresh();

		Debug.Log($"更新HotFix.dll!{_srcDllPath}-->{_destDllPath}");
	}

}
