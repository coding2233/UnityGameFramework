//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #ui管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 10点59分# </time>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix.Taurus
{
    public sealed class UIManager : GameFrameworkModule,IUpdate
    {
        //事件管理器
        //资源管理器
        private GameFramework.Taurus.ResourceManager _resource;
        //ui堆栈
        private Stack<AssetConfig> _stackUiAsset = new Stack<AssetConfig>();
        //所有的uiview
        private readonly Dictionary<AssetConfig, UIView> _allUiViews = new Dictionary<AssetConfig, UIView>();
        //默认的uipath路径
        private readonly Dictionary<int, AssetConfig> _uiAssetPath = new Dictionary<int, AssetConfig>();
        //所有的uiAsset
        private readonly Dictionary<int, AssetConfig> _allUiAssets = new Dictionary<int, AssetConfig>();
        //带Update的UIView
        private List<IUpdate> _allUpdates;
        #region 构造函数
        public UIManager()
        {
            //获取资源模块
            _resource = GameFramework.Taurus.GameFrameworkMode.GetModule<GameFramework.Taurus.ResourceManager>();
            _allUpdates=new List<IUpdate>();
        }
        #endregion

        #region 外部接口
        public async void Push<T>(bool allowMulti = false, params object[] parameters) where T : UIView
        {
            AssetConfig assetConfig = CheckAssetPath(typeof(T));
            if (assetConfig == null)
                return;
            
            if (_stackUiAsset.Count > 0)
            {
                AssetConfig lastAssetCofig = _stackUiAsset.Peek();
                //如果界面已经打开 则不在执行
                if (Equals(lastAssetCofig, assetConfig) && !allowMulti)
                    return;
                IUIView uiView = await GetUiView<T>(lastAssetCofig);

                ////触发暂停事件
                //_uiPauseArgs.UIView = uiView;
                //_event.Trigger(this, _uiPauseArgs);

                uiView.OnPause();
            }

            AssetConfig newAssetConfig = null;
            if (allowMulti)
                newAssetConfig = new AssetConfig(assetConfig.AssetBundleName, assetConfig.AssetPath);
            else
                newAssetConfig = assetConfig;


            _stackUiAsset.Push(newAssetConfig);
            UIView newUiView = await GetUiView<T>(newAssetConfig);
            newUiView.OnEnter(parameters);
            //更新函数 -- 添加
            IUpdate update = newUiView as IUpdate;
            if (update!=null)
            {
                _allUpdates.Add(update);
            }
            ////触发打开事件
            //_uiEnterArgs.UIView = newUiView;
            //_event.Trigger(this, _uiEnterArgs);
        }


        public void Pop(bool isDestory = false)
        {
            //移除当前UI
            if (_stackUiAsset.Count > 0)
            {
                AssetConfig lastAssetConfig = _stackUiAsset.Pop();
                UIView lastUiView;
                if (_allUiViews.TryGetValue(lastAssetConfig, out lastUiView))
                {
                    //更新函数  -- 移除
                    IUpdate update = lastUiView as IUpdate;
                    if (update!=null&&_allUpdates.Contains(update))
                    {
                        _allUpdates.Remove(update);
                    }
                    ////触发关闭事件
                    //_uiExitArgs.UIView = lastUiView;
                    //_event.Trigger(this, _uiExitArgs);

                    lastUiView.OnExit();
                    if (isDestory)
                    {
                        _allUiViews.Remove(lastAssetConfig);
                        MonoBehaviour.Destroy(lastUiView.gameObject);
                    }
                    else
                        lastUiView.gameObject.SetActive(false);
                }
            }

            if (_stackUiAsset.Count > 0)
            {
                AssetConfig lastAssetConfig = _stackUiAsset.Peek();
				UIView lastUiView;
				if (_allUiViews.TryGetValue(lastAssetConfig, out lastUiView))
                    lastUiView.OnResume();

                ////触发恢复事件
                //_uiResumeArgs.UIView = lastUiView;
                //_event.Trigger(this, _uiResumeArgs);
            }
        }

        #endregion



        #region 内部函数
        //更新函数
        public void OnUpdate()
        {
            if (_allUiViews != null)
            {
                for (int i = 0; i < _allUpdates.Count; i++)
                {
                    _allUpdates[i].OnUpdate();
                }
//                foreach (var item in _allUiViews.Values)
//                    item.OnUpdate();
            }
        }

        //检查路径
        private AssetConfig CheckAssetPath(Type t)
        {
            int hashCode = t.GetHashCode();
			AssetConfig assetCofig;
			if (!_uiAssetPath.TryGetValue(hashCode, out assetCofig))
            {
                object[] attrs = t.GetCustomAttributes(typeof(UIViewAttribute), false);
                if (attrs.Length == 0)
                    return null;
				UIViewAttribute uIViewAttribute = attrs[0] as UIViewAttribute;
				if (uIViewAttribute==null || string.IsNullOrEmpty(uIViewAttribute.ViewPath)
                    || string.IsNullOrEmpty(uIViewAttribute.AssetBundleName))
                    return null;
                assetCofig = new AssetConfig(uIViewAttribute.AssetBundleName, uIViewAttribute.ViewPath);
				_uiAssetPath.Add(hashCode, assetCofig);
			}
            return assetCofig;
        }


        //获取ui界面
        private async Task<UIView> GetUiView<T>(AssetConfig assetConfig) where T:UIView
        {
            UIView uiView;
            if (!_allUiViews.TryGetValue(assetConfig, out uiView))
            {
                GameObject uiViewSource = await _resource.LoadAsset<GameObject>(assetConfig.AssetBundleName, assetConfig.AssetPath);
                if (uiViewSource == null)
                    return null;
                GameObject uiViewClone = GameObject.Instantiate(uiViewSource);
                //ILRuntime不支持带参数构造函数的生成 
                // uiView= (UIView)Activator.CreateInstance(typeof(T), new object[] { uiViewClone });
                //用反射调用构造函数生成对象
                //根据参数类型获取构造函数 
                ConstructorInfo ci = typeof(T).GetConstructor(new Type[]{typeof(GameObject)});
                object[] obj = new object[1] { uiViewClone };
                if (ci != null) uiView = ci.Invoke(obj) as UIView;
                if (uiView == null)
                    return null;
                _allUiViews[assetConfig] = uiView;
                return uiView;
            }
            uiView.gameObject.SetActive(true);
            return uiView;
        }

        #endregion


        #region 重写函数
        public override void OnClose()
        {
            _stackUiAsset.Clear();
            _allUiAssets.Clear();
            _allUiAssets.Clear();

            foreach (var item in _allUiViews.Values)
            {
                MonoBehaviour.Destroy(item.gameObject);
            }
            _allUiViews.Clear();
        }
        #endregion


        private class AssetConfig
        {
            public string AssetBundleName;
            public string AssetPath;

            public AssetConfig()
            {
            }

            public AssetConfig(string assetBundleName, string assetPath)
            {
                AssetBundleName = assetBundleName;
                AssetPath = assetPath;
            }
        }


    }
}