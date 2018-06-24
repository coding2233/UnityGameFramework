//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #可操作物体的配置类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点39分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
	public sealed class OperationAssetConfig : MonoBehaviour
	{
		/// <summary>
		/// 状态变化事件
		/// </summary>
		public UnityIntEvent OnStatusChanged = new UnityIntEvent();

		/// <summary>
		/// 当前设备的ID
		/// </summary>  
		public long OperationId;

		/// <summary>
		/// 设备的状态
		/// </summary>
		[HideInInspector]
		[SerializeField]
		private int _status = 1;
		public int Status
		{
			get { return _status; }
			set
			{
				//可以执行一系列其他操作
				_status = value;
				//触发事件
				OnStatusChanged.Invoke(_status);
			}
		}
		/// <summary>
		/// 是否使用设备检测过
		/// </summary>
		public bool Checked { get; set; }
	}
}
