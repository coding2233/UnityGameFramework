//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #ui显示的接口类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 10点56分# </time>
//-----------------------------------------------------------------------


namespace HotFix.Taurus
{
    public interface IUIView
    {
        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="parameters">不确定参数</param>
        void OnEnter(params object[] parameters);
        /// <summary>
        /// 退出界面
        /// </summary>
        void OnExit();
        /// <summary>
        /// 暂停界面
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复界面
        /// </summary>
        void OnResume();

    }
}