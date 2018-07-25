//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Hu Tao. All rights reserved.
// </copyright>
// <describe> #调试管理器# </describe>
// <email> 987947865@qq.com </email>
// <time> #2018年7月25日 15点38分# </time>
//-----------------------------------------------------------------------


namespace GameFramework.Taurus
{
    public class DebugManager : GameFrameworkModule
    {
        private bool _enable = true;
        private DebugHelper _helper;

        /// <summary>
        /// 调试器可见性
        /// </summary>
        public bool Enable
        {
            get
            {
                return _enable;
            }
            set
            {
                _enable = value;
                _helper.enabled = _enable;
            }
        }

        /// <summary>
        /// 设置调试器帮助类
        /// </summary>
        public void SetDebugHelper(DebugHelper helper)
        {
            _helper = helper;
        }

        public override void OnClose()
        {
            
        }
    }
}
