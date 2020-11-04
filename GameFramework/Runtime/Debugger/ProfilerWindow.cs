using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [DebuggerWindow("Profiler")]
    public class ProfilerWindow : ToolbarDebuggerWindow
    {
        public override void OnInit(params object[] args)
        {
            base.OnInit(args);

            var windows = new IDebuggerWindow[] { new SummaryProfiler(), new MemoryProfiler() };
            var windowTitle = new string[windows.Length];
            for (int i = 0; i < windowTitle.Length; i++)
            {
                windowTitle[i] = windows[i].GetType().Name.Replace("Profiler", "");
            }
            //设置子窗口
            SetChildWindows(windows, windowTitle, args);
        }

    }

    //概要分析
    internal class SummaryProfiler : IDebuggerWindow
    {
        public void OnInit(params object[] args)
        {
        }

        public void OnClose()
        {
        }

        public void OnDraw()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }

    //内存分析
    internal class MemoryProfiler : ToolbarDebuggerWindow
    {
        public override void OnInit(params object[] args)
        {
            base.OnInit(args);

            var windows = new IDebuggerWindow[] {new SummaryMemoryProfiler()
            ,new AllMemoryProfiler(),new TextureMemoryProfiler(),new MeshMemoryProfiler(),new MaterialMemoryProfiler(),new ShaderMemoryProfiler()
            ,new AnimationClipMemoryProfiler(),new AudioClipMemoryProfiler(),new FontMemoryProfiler(),new TextAssetMemoryProfiler(),new ScriptableObjectMemoryProfiler() };
            var windowTitle = new string[windows.Length];
            for (int i = 0; i < windowTitle.Length; i++)
            {
                windowTitle[i] = windows[i].GetType().Name.Replace("MemoryProfiler", "");
            }
            //设置子窗口
            SetChildWindows(windows, windowTitle, args);
        }
    }

    //内存分析基类
    internal abstract class MemoryProfilerBase : IDebuggerWindow
    {
        Vector2 _scrollPos = Vector2.zero;
        public virtual void OnInit(params object[] args)
        {
        }
        public virtual void OnClose()
        {
        }


        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnDraw()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");
            if (GUILayout.Button("Take Sample"))
            {
                TakeSample();
            }
            OnScrollViewDraw();
            GUILayout.EndScrollView();
        }

        protected abstract void OnScrollViewDraw();

        protected abstract void TakeSample();

    }
    //概要内存分析
    internal class SummaryMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //所有的内存分析
    internal class AllMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //Texture内存分析
    internal class TextureMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //Mesh内存分析
    internal class MeshMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //Material内存分析
    internal class MaterialMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //Shader内存分析
    internal class ShaderMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //AnimationClip内存分析
    internal class AnimationClipMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //AudioClip内存分析
    internal class AudioClipMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    // Font内存分析
    internal class FontMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //TextAsset内存分析
    internal class TextAssetMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }
    //ScriptableObject内存分析
    internal class ScriptableObjectMemoryProfiler : MemoryProfilerBase
    {
        protected override void OnScrollViewDraw()
        {
        }

        protected override void TakeSample()
        {
        }
    }

}

