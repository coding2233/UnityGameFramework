//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #ui管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 17点01分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
    public sealed class UIManager :GameFrameworkModule
    {
		//事件管理器
		private EventManager _event;
		private UIEnterEventArgs _uiEnterArgs;
		private UIExitEventArgs _uiExitArgs;
		private UIPauseEventArgs _uiPauseArgs;
		private UIResumeEventArgs _uiResumeArgs;

		//资源管理器
		private ResourceManager _resource;
		//ui堆栈
		private Stack<AssetConfig> _stackUiAsset = new Stack<AssetConfig>();
		//所有的uiview
		private readonly Dictionary<AssetConfig, UIView> _allUiViews = new Dictionary<AssetConfig, UIView>();
		//默认的uipath路径
        private readonly Dictionary<int, AssetConfig> _uiAssetPath =
            new Dictionary<int, AssetConfig>();
		//所有的uiAsset
		private readonly Dictionary<int, AssetConfig> _allUiAssets = new Dictionary<int, AssetConfig>();
		#region 构造函数
		public UIManager()
		{
			//获取资源模块
			_resource = GameFrameworkMode.GetModule<ResourceManager>();
			//获取事件模块
			_event = GameFrameworkMode.GetModule<EventManager>();

			_uiEnterArgs = new UIEnterEventArgs();
			_uiExitArgs = new UIExitEventArgs();
			_uiPauseArgs = new UIPauseEventArgs();
			_uiResumeArgs = new UIResumeEventArgs();
		}
		#endregion

		#region 外部接口
		public void Push<T>(bool allowMulti = false, params object[] parameters) where T : UIView
		{
		    AssetConfig assetConfig = CheckAssetPath(typeof(T));
			if (assetConfig==null)
				return;
            
			if (_stackUiAsset.Count > 0)
			{
			    AssetConfig lastAssetConfig = _stackUiAsset.Peek();
				//如果界面已经打开 则不在执行
				if (Equals(lastAssetConfig, assetConfig) && !allowMulti)
					return;

				IUIView uiView = GetUiView(lastAssetConfig);

				//触发暂停事件
				_uiPauseArgs.UIView = uiView;
				_event.Trigger(this, _uiPauseArgs);

				uiView.OnPause();
			}

		    AssetConfig newAssetConfig = null;
		    if (allowMulti)
		        newAssetConfig = new AssetConfig(assetConfig.AssetBundleName, assetConfig.AssetPath);
		    else
		        newAssetConfig = assetConfig;

            _stackUiAsset.Push(newAssetConfig);
			UIView newUiView = GetUiView(newAssetConfig);
			newUiView.OnEnter(parameters);

			//触发打开事件
			_uiEnterArgs.UIView = newUiView;
			_event.Trigger(this, _uiEnterArgs);
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
					//触发关闭事件
					_uiExitArgs.UIView = lastUiView;
					_event.Trigger(this, _uiExitArgs);

					lastUiView.OnExit();
					if (isDestory)
					{
						_allUiViews.Remove(lastAssetConfig);
						MonoBehaviour.Destroy(lastUiView);
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
			    {
			        lastUiView.OnResume();
			        //触发恢复事件
			        _uiResumeArgs.UIView = lastUiView;
			        _event.Trigger(this, _uiResumeArgs);
                }
			}
		}

		#endregion



		#region 内部函数
		//检查路径
		private AssetConfig CheckAssetPath(Type t)
		{
		    int hashCode = t.GetHashCode();

		    AssetConfig assetCofig = null;
            if (!_uiAssetPath.TryGetValue(hashCode, out assetCofig))
		    {
		        object[] attrs = t.GetCustomAttributes(typeof(UIViewAttribute), false);
		        if (attrs.Length == 0)
		            return null;
		        UIViewAttribute uIViewAttribute = attrs[0] as UIViewAttribute;

                if (string.IsNullOrEmpty(uIViewAttribute?.ViewPath) || string.IsNullOrEmpty(uIViewAttribute.AssetBundleName))
		            return null;
		        assetCofig = new AssetConfig(uIViewAttribute.AssetBundleName, uIViewAttribute.ViewPath);

				_uiAssetPath[hashCode] = assetCofig;
			}
		    return assetCofig;
        }


		//获取ui界面
		private UIView GetUiView(AssetConfig assetConfig)
		{
			UIView uiView;
			if (!_allUiViews.TryGetValue(assetConfig, out uiView))
			{
				GameObject uiViewSource = _resource.LoadAsset<GameObject>(assetConfig.AssetBundleName, assetConfig.AssetPath);
				if (uiViewSource == null)
					throw new Exception("uiview path not found:"+ assetConfig.AssetBundleName+":"+ assetConfig.AssetPath);

				GameObject uiViewClone = GameObject.Instantiate(uiViewSource);
				uiView = uiViewClone.GetComponent<UIView>();
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
				MonoBehaviour.Destroy(item);
			}
			_allUiViews.Clear();
		}
        #endregion


        #region 数据结构
        //资源配置
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
        #endregion

    }
}
