//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏状态基类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 16点47分# </time>
//-----------------------------------------------------------------------

namespace GameFramework.Taurus
{
    public abstract class GameState
    {
        #region 属性
        private GameStateContext _context;

        internal void SetStateContext(GameStateContext context)
        {
            _context = context;
        }

        #endregion


        #region
        /// <summary>
        /// 初始化 -- 只执行一次
        /// </summary>
        public virtual void OnInit()
        {
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="parameters">不确定参数</param>
        public virtual void OnEnter(params object[] parameters)
        {

        }

        /// <summary>
        /// 退出状态
        /// </summary>
        public virtual void OnExit()
        {
        }

        /// <summary>
        /// 渲染帧函数
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 固定帧函数
        /// </summary>
        public virtual void OnFixedUpdate()
        {
        }


        #endregion

        #region 内部函数

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="T">游戏状态</typeparam>
        protected void ChangeState<T>(params object[] parameters) where T : GameState
        {
            if (_context !=null)
                _context.ChangeState<T>(parameters);
        }

        #endregion

    }
}
