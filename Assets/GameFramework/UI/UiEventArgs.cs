//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #ui事件参数# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 17点38分# </time>
//-----------------------------------------------------------------------

namespace GameFramework.Taurus
{

	/// <summary>
	/// ui打开事件
	/// </summary>
	public class UIEnterEventArgs : GameEventArgs<UIEnterEventArgs>
	{
		public IUIView UIView;
	}

	/// <summary>
	/// ui关闭事件
	/// </summary>
	public class UIExitEventArgs : GameEventArgs<UIExitEventArgs>
	{
		public IUIView UIView;
	}

	/// <summary>
	/// ui暂停事件
	/// </summary>
	public class UIPauseEventArgs : GameEventArgs<UIPauseEventArgs>
	{
		public IUIView UIView;
	}


	/// <summary>
	/// ui恢复事件
	/// </summary>
	public class UIResumeEventArgs : GameEventArgs<UIResumeEventArgs>
	{
		public IUIView UIView;
	}

}