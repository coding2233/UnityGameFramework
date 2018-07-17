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
using UnityEngine;

namespace HotFix.Taurus
{
    public sealed class UIManager : GameFrameworkModule,IUpdate
    {
        //事件管理器
        //资源管理器
        private GameFramework.Taurus.ResourceManager _resource;
        //ui堆栈
        private Stack<UiAsset> _stackUiAsset = new Stack<UiAsset>();
        //所有的uiview
        private readonly Dictionary<UiAsset, UIView> _allUiViews = new Dictionary<UiAsset, UIView>();
        //默认的uipath路径
        private readonly Dictionary<int, string> _uiAssetPath = new Dictionary<int, string>();
        //所有的uiAsset
        private readonly Dictionary<int, UiAsset> _allUiAssets = new Dictionary<int, UiAsset>();
        #region 构造函数
        public UIManager()
        {
            //获取资源模块
            _resource = GameFramework.Taurus.GameFrameworkMode.GetModule<GameFramework.Taurus.ResourceManager>();
        }
        #endregion

        #region 外部接口
        public void Push<T>(bool allowMulti = false, params object[] parameters) where T : UIView
        {
            string assetPath = CheckAssetPath(typeof(T));
            if (string.IsNullOrEmpty(assetPath))
                return;

            int hashCode = typeof(T).GetHashCode();

            if (_stackUiAsset.Count > 0)
            {
                UiAsset uiAsset = _stackUiAsset.Peek();
                //如果界面已经打开 则不在执行
                if (uiAsset.AssetPath == assetPath && !allowMulti)
                    return;
                IUIView uiView = GetUiView<T>(uiAsset);

                ////触发暂停事件
                //_uiPauseArgs.UIView = uiView;
                //_event.Trigger(this, _uiPauseArgs);

                uiView.OnPause();
            }

            UiAsset newUiAsset = null;
            if (!allowMulti)
            {
                if (!_allUiAssets.TryGetValue(hashCode, out newUiAsset))
                {
                    newUiAsset = new UiAsset(assetPath);
                    _allUiAssets[hashCode] = newUiAsset;
                }
            }
            else
                newUiAsset = new UiAsset(assetPath);

            _stackUiAsset.Push(newUiAsset);
            UIView newUiView = GetUiView<T>(newUiAsset);
            newUiView.OnEnter(parameters);

            ////触发打开事件
            //_uiEnterArgs.UIView = newUiView;
            //_event.Trigger(this, _uiEnterArgs);
        }


        public void Pop(bool isDestory = false)
        {
            //移除当前UI
            if (_stackUiAsset.Count > 0)
            {
                UiAsset lastUiAsset = _stackUiAsset.Pop();
                UIView lastUiView;
                if (_allUiViews.TryGetValue(lastUiAsset, out lastUiView))
                {
                    ////触发关闭事件
                    //_uiExitArgs.UIView = lastUiView;
                    //_event.Trigger(this, _uiExitArgs);

                    lastUiView.OnExit();
                    if (isDestory)
                    {
                        _allUiViews.Remove(lastUiAsset);
                        MonoBehaviour.Destroy(lastUiView.gameObject);
                    }
                    else
                        lastUiView.gameObject.SetActive(false);
                }
            }

            if (_stackUiAsset.Count > 0)
            {
                UiAsset uiAsset = _stackUiAsset.Peek();
                if (_allUiViews.TryGetValue(uiAsset, out var lastUiView))
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
                foreach (var item in _allUiViews.Values)
                    item.OnUpdate();
            }
        }

        //检查路径
        private string CheckAssetPath(Type t)
        {
            int hashCode = t.GetHashCode();

            string assetPath;
            if (!_uiAssetPath.TryGetValue(hashCode, out assetPath))
            {
                object[] attrs = t.GetCustomAttributes(typeof(UIViewAttribute), false);
                if (attrs == null || attrs.Length == 0)
                    return "";
                UIViewAttribute uiViewAttribute = (UIViewAttribute)attrs[0];
                if (string.IsNullOrEmpty(uiViewAttribute.ViewPath))
                    return "";

                assetPath = uiViewAttribute.ViewPath;
                _uiAssetPath[hashCode] = uiViewAttribute.ViewPath;
            }
            return assetPath;
        }


        //获取ui界面
        private UIView GetUiView<T>(UiAsset uiAsset) where T:UIView
        {
            UIView uiView;
            if (!_allUiViews.TryGetValue(uiAsset, out uiView))
            {
                GameObject uiViewSource = _resource.LoadAsset<GameObject>(uiAsset.AssetPath);
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
                _allUiViews[uiAsset] = uiView;
                return _allUiViews[uiAsset];
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

        #region 数据结构
        //ui的类型
        private sealed class UiAsset
        {
            public string AssetPath { get; private set; }

            public UiAsset(string path)
            {
                AssetPath = path;
            }
        }
        #endregion


    }
}