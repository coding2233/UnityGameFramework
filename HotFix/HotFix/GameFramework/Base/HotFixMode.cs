namespace HotFix.Taurus
{
    public class HotFixMode
    {
        #region 属性
        public static GameStateManager State;
        public static EventManager Event;
        public static UIManager UI;
		public static DataTableManager DataTable;
        #endregion

        public HotFixMode()
        {
	        UnityEngine.Debug.Log("-----HotFix.Taurus.HotFixMode Hello!!!-----");

            #region 获取热更新组件
            State = GameFrameworkMode.GetModule<GameStateManager>();
            Event = GameFrameworkMode.GetModule<EventManager>();
            UI = GameFrameworkMode.GetModule<UIManager>();
	        DataTable = GameFrameworkMode.GetModule<DataTableManager>();
			#endregion

			//从主框架中获取热更新模块
			GameFramework.Taurus.HotFixManager hotFixManager =
                GameFramework.Taurus.GameFrameworkMode.GetModule<GameFramework.Taurus.HotFixManager>();

            #region 开启热更新流程
            ////开启整个项目的流程
            State.CreateContext(hotFixManager.GetHotFixTypes.ToArray());
            State.SetStateStart();
            #endregion

            #region 添加回调
            hotFixManager.Update += OnUpdate;
            hotFixManager.FixedUpdate += OnFixedUpdate;
            hotFixManager.Close += OnClose;
            #endregion
        }

        private void OnUpdate()
        {
            GameFrameworkMode.Update();
        }

        private void OnFixedUpdate()
        {
            GameFrameworkMode.FixedUpdate();
        }

        private void OnClose()
        {
            GameFrameworkMode.ShutDown();
        }

    }
}