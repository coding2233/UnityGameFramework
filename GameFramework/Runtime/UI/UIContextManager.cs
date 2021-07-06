using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class UIContextManager
	{
        //所有的UIContext
        private readonly Dictionary<string, IUIContext> _allUIContexts = new Dictionary<string, IUIContext>();
        private readonly Dictionary<string, Queue<IUIContext>> _allMultipleUIContexts = new Dictionary<string, Queue<IUIContext>>();

        /// <summary>
        /// 添加UIContext
        /// </summary>
        /// <param name="allUIContexts"></param>
        public void AddUIContext(IUIContext[] allUIContexts)
        {
            for (int i = 0; i < allUIContexts.Length; i++)
            {
                _allUIContexts[allUIContexts[i].AssetPath]= allUIContexts[i];
            }
        }

        /// <summary>
        /// 添加UIContext
        /// </summary>
        /// <param name="allUIContexts"></param>
        public void AddUIContext(IUIContext uiContext)
        {
            _allUIContexts[uiContext.AssetPath] = uiContext;
        }

        public IUIContext this[string assetPath]
        {
            get 
            {
                IUIContext uiContext = null;
                _allUIContexts.TryGetValue(assetPath, out uiContext);
                if (uiContext != null)
                {
                    if (uiContext.Multiple)
                    {
                        uiContext = GetMultipleUIContext(uiContext);
                    }
                }
                else
                {
                    //默认使用UIContextBase 不支持多个
                    UIContextBase newUIContext = new UIContextBase();
                    newUIContext.AssetPath = assetPath;
                    newUIContext.Name = Path.GetFileNameWithoutExtension(assetPath);
                    newUIContext.Multiple = false;
                    uiContext = newUIContext;
                    _allUIContexts[assetPath] = newUIContext;
                }
                return uiContext;
            }
        }

        private IUIContext GetMultipleUIContext(IUIContext nativeUIContext)
        {
            string assetPath = nativeUIContext.AssetPath;
            IUIContext uiContext = null;
            if (_allMultipleUIContexts.TryGetValue(assetPath, out Queue<IUIContext> queueUIContext))
            {
                if (queueUIContext != null && queueUIContext.Count > 0)
                {
                    uiContext = queueUIContext.Dequeue();
                }
            }
            else
            {
                uiContext = nativeUIContext.Clone();
            }
            return uiContext;
        }

        /// <summary>
        /// 回收重复出现的UIContext
        /// </summary>
        /// <param name="uiContext"></param>
        public void ReleaseUIContext(IUIContext uiContext)
        {
            string assetPath = uiContext.AssetPath;
            if (uiContext != null && uiContext.Multiple)
            {
                Queue<IUIContext> queueUIContext;
                if (_allMultipleUIContexts.TryGetValue(assetPath, out  queueUIContext))
                {
                    queueUIContext.Enqueue(uiContext);
                }
                else
                {
                    queueUIContext = new Queue<IUIContext>();
                    queueUIContext.Enqueue(uiContext);
                    _allMultipleUIContexts[assetPath] = queueUIContext;
                }
            }
        }

    }
}