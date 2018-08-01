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
        private Stack<UIViewAttribute> _stackUiAsset = new Stack<UIViewAttribute>();
        //所有的uiview
        private readonly Dictionary<UIViewAttribute, UIView> _allUiViews = new Dictionary<UIViewAttribute, UIView>();
        //默认的uipath路径
        private readonly Dictionary<int, UIViewAttribute> _uiAssetPath = new Dictionary<int, UIViewAttribute>();
        //所有的uiAsset
        private readonly Dictionary<int, UIViewAttribute> _allUiAssets = new Dictionary<int, UIViewAttribute>();
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
            UIViewAttribute uiViewAttribute = CheckAssetPath(typeof(T));
            if (uiViewAttribute == null)
                return;
            
            if (_stackUiAsset.Count > 0)
            {
                UIViewAttribute lastUiViewAttribute = _stackUiAsset.Peek();
                //如果界面已经打开 则不在执行
                if (Equals(lastUiViewAttribute, uiViewAttribute) && !allowMulti)
                    return;
                IUIView uiView = GetUiView<T>(lastUiViewAttribute);

                ////触发暂停事件
                //_uiPauseArgs.UIView = uiView;
                //_event.Trigger(this, _uiPauseArgs);

                uiView.OnPause();
            }

            UIViewAttribute newUIViewAttribute = uiViewAttribute;
            if (allowMulti)
                newUIViewAttribute = new UIViewAttribute(uiViewAttribute.AssetBundleName, uiViewAttribute.ViewPath);


            _stackUiAsset.Push(newUIViewAttribute);
            UIView newUiView = GetUiView<T>(newUIViewAttribute);
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
                UIViewAttribute lastUIViewAttribute = _stackUiAsset.Pop();
                UIView lastUiView;
                if (_allUiViews.TryGetValue(lastUIViewAttribute, out lastUiView))
                {
                    ////触发关闭事件
                    //_uiExitArgs.UIView = lastUiView;
                    //_event.Trigger(this, _uiExitArgs);

                    lastUiView.OnExit();
                    if (isDestory)
                    {
                        _allUiViews.Remove(lastUIViewAttribute);
                        MonoBehaviour.Destroy(lastUiView.gameObject);
                    }
                    else
                        lastUiView.gameObject.SetActive(false);
                }
            }

            if (_stackUiAsset.Count > 0)
            {
                UIViewAttribute uiViewAttribute = _stackUiAsset.Peek();
                if (_allUiViews.TryGetValue(uiViewAttribute, out var lastUiView))
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
        private UIViewAttribute CheckAssetPath(Type t)
        {
            int hashCode = t.GetHashCode();

            UIViewAttribute uiViewAttribute = null;
            if (!_uiAssetPath.TryGetValue(hashCode, out uiViewAttribute))
            {
                object[] attrs = t.GetCustomAttributes(typeof(UIViewAttribute), false);
                if (attrs.Length == 0)
                    return null;
                uiViewAttribute = (UIViewAttribute)attrs[0];
                if (string.IsNullOrEmpty(uiViewAttribute.ViewPath)
                    || string.IsNullOrEmpty(uiViewAttribute.AssetBundleName))
                    return null;
            }
            return uiViewAttribute;
        }


        //获取ui界面
        private UIView GetUiView<T>(UIViewAttribute uiViewAttribute) where T:UIView
        {
            UIView uiView;
            if (!_allUiViews.TryGetValue(uiViewAttribute, out uiView))
            {
                GameObject uiViewSource = _resource.LoadAsset<GameObject>(uiViewAttribute.AssetBundleName, uiViewAttribute.ViewPath);
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
                _allUiViews[uiViewAttribute] = uiView;
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
        


    }
}