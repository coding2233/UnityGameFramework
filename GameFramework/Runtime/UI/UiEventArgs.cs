//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #ui事件参数# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月22日 17点38分# </time>
//-----------------------------------------------------------------------

namespace Wanderer.GameFramework
{

    /// <summary>
    /// ui打开事件
    /// </summary>
    public class UIEnterEventArgs : GameEventArgs<UIEnterEventArgs>
    {
        public UIView UIView;
    }

    /// <summary>
    /// ui关闭事件
    /// </summary>
    public class UIExitEventArgs : GameEventArgs<UIExitEventArgs>
    {
        public UIView UIView;
    }

    /// <summary>
    /// ui暂停事件
    /// </summary>
    public class UIPauseEventArgs : GameEventArgs<UIPauseEventArgs>
    {
        public UIView UIView;
    }


    /// <summary>
    /// ui恢复事件
    /// </summary>
    public class UIResumeEventArgs : GameEventArgs<UIResumeEventArgs>
    {
        public UIView UIView;
    }

}