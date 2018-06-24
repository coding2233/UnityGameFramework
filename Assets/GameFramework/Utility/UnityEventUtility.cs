//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #基本类型unity事件的定义# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点41分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.Taurus
{
	[System.Serializable]
	public class UnityIntEvent : UnityEvent<int>
	{

	}

	[System.Serializable]
	public class UnityStringEvent : UnityEvent<string>
	{

	}

	[System.Serializable]
	public class UnityFloatEvent : UnityEvent<float>
	{
	}

	[System.Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}
}
