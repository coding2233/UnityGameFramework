using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace Wanderer.GameFramework
{
    [DebuggerWindow("Profiler")]
    public class ProfilerWindow : ToolbarDebuggerWindow
    {
        private MemoryProfiler _mpWindow;

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

            //内存窗口
            _mpWindow = windows[1] as MemoryProfiler;
        }

		public override void OnDraw()
		{
			base.OnDraw();
		}

        /// <summary>
        /// 自动进行性能测试
        /// </summary>
        /// <param name="totalSeconds"></param>
        /// <param name="intervalSeconds"></param>
        public async void AutoTakeSample(int totalSeconds=30,int intervalSeconds=5)
        {
            if (_mpWindow != null)
            {
                Log.Info($"[*] [MemoryProfiler] [AutoTakeSample] [****************************************]");
                int second = 0;
                while (second < totalSeconds)
                {
                    _mpWindow.AutoTakeSample();
                    await Task.Delay(intervalSeconds * 1000);
                    second += intervalSeconds;
                }
                Log.Info($"[*] [MemoryProfiler] [AutoTakeSample] [****************************************]");
            }
        }

	}

    //概要分析
    internal class SummaryProfiler : DebuggerWindowBase
    {
        public override void OnDraw()
        {
            GUILayout.Label("<b>Profiler Information</b>");
            GUILayout.BeginVertical("box");
            {
                GuiUtility.DrawItem("Supported", Profiler.supported.ToString());
                GuiUtility.DrawItem("Enabled", Profiler.enabled.ToString());
                GuiUtility.DrawItem("Enable Binary Log", Profiler.enableBinaryLog ? string.Format("True, {0}", Profiler.logFile) : "False");
#if UNITY_2018_3_OR_NEWER
                GuiUtility.DrawItem("Area Count", Profiler.areaCount.ToString());
#endif
#if UNITY_5_3 || UNITY_5_4
                GuiUtility.DrawItem("Max Samples Number Per Frame", Profiler.maxNumberOfSamplesPerFrame.ToString());
#endif
#if UNITY_2018_3_OR_NEWER
                GuiUtility.DrawItem("Max Used Memory", Profiler.maxUsedMemory.ToByteLengthString());
#endif
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Mono Used Size", Profiler.GetMonoUsedSizeLong().ToByteLengthString());
                GuiUtility.DrawItem("Mono Heap Size", Profiler.GetMonoHeapSizeLong().ToByteLengthString());
                GuiUtility.DrawItem("Used Heap Size", Profiler.usedHeapSizeLong.ToByteLengthString());
                GuiUtility.DrawItem("Total Allocated Memory", Profiler.GetTotalAllocatedMemoryLong().ToByteLengthString());
                GuiUtility.DrawItem("Total Reserved Memory", Profiler.GetTotalReservedMemoryLong().ToByteLengthString());
                GuiUtility.DrawItem("Total Unused Reserved Memory", Profiler.GetTotalUnusedReservedMemoryLong().ToByteLengthString());
#else
                GuiUtility.DrawItem("Mono Used Size", GetByteLengthString(Profiler.GetMonoUsedSize()));
                GuiUtility.DrawItem("Mono Heap Size", GetByteLengthString(Profiler.GetMonoHeapSize()));
                GuiUtility.DrawItem("Used Heap Size", GetByteLengthString(Profiler.usedHeapSize));
                GuiUtility.DrawItem("Total Allocated Memory", GetByteLengthString(Profiler.GetTotalAllocatedMemory()));
                GuiUtility.DrawItem("Total Reserved Memory", GetByteLengthString(Profiler.GetTotalReservedMemory()));
                GuiUtility.DrawItem("Total Unused Reserved Memory", GetByteLengthString(Profiler.GetTotalUnusedReservedMemory()));
#endif
#if UNITY_2018_1_OR_NEWER
                GuiUtility.DrawItem("Allocated Memory For Graphics Driver", Profiler.GetAllocatedMemoryForGraphicsDriver().ToByteLengthString());
#endif
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Temp Allocator Size", Profiler.GetTempAllocatorSize().ToByteLengthString());
                //  GuiUtility.DrawItem("Marshal Cached HGlobal Size", Utility.Marshal.CachedHGlobalSize.);
                //     GuiUtility.DrawItem("Data Provider Cached Bytes Size", DataProviderCreator.CachedBytesSize);
#endif
            }
            GUILayout.EndVertical();
        }


        public static string ProfilerToString()
        {
            Log.StrBuilder.Clear();
            Log.StrBuilder.AppendLine($"[*] [SummaryProfiler]");
            Log.StrBuilder.AppendLine("{");
            Log.StrBuilder.AppendLine($"    Profiler.supported:{Profiler.supported}");
            Log.StrBuilder.AppendLine($"    Profiler.enabled:{Profiler.enabled}");
            Log.StrBuilder.AppendLine($"    Profiler.enableBinaryLog:{Profiler.enableBinaryLog}");
            Log.StrBuilder.AppendLine($"    Profiler.logFile:{Profiler.logFile}");
            Log.StrBuilder.AppendLine($"    Profiler.areaCount:{Profiler.areaCount}");
            Log.StrBuilder.AppendLine($"    Profiler.maxUsedMemory:{Profiler.maxUsedMemory.ToByteLengthString()}");
            Log.StrBuilder.AppendLine($"    Profiler.GetMonoUsedSizeLong():{ Profiler.GetMonoUsedSizeLong().ToByteLengthString()}");
            Log.StrBuilder.AppendLine($"    Profiler.GetMonoHeapSizeLong():{ Profiler.GetMonoHeapSizeLong().ToByteLengthString()}");
            Log.StrBuilder.AppendLine($"    Profiler.usedHeapSizeLong:{ Profiler.usedHeapSizeLong.ToByteLengthString()}");
            Log.StrBuilder.AppendLine($"    Profiler.GetTotalAllocatedMemoryLong():{ Profiler.GetTotalAllocatedMemoryLong().ToByteLengthString()}");
            Log.StrBuilder.AppendLine($"    Profiler.GetTotalReservedMemoryLong():{ Profiler.GetTotalReservedMemoryLong().ToByteLengthString()}");
            Log.StrBuilder.AppendLine($"    Profiler.GetTotalUnusedReservedMemoryLong():{ Profiler.GetTotalUnusedReservedMemoryLong().ToByteLengthString()}");
            Log.StrBuilder.AppendLine($"    Profiler.GetAllocatedMemoryForGraphicsDriver():{ Profiler.GetAllocatedMemoryForGraphicsDriver().ToByteLengthString()}");
            Log.StrBuilder.AppendLine($"    Profiler.GetTempAllocatorSize():{ Profiler.GetTempAllocatorSize().ToByteLengthString()}");
            Log.StrBuilder.AppendLine();
            Log.StrBuilder.AppendLine("}");
            string info = Log.StrBuilder.ToString();
            Log.Info(info);
            return info;
        }

    }

    //内存分析
    internal class MemoryProfiler : ToolbarDebuggerWindow
    {
        public override void OnDraw()
        {
            // base.OnDraw();
            int selectIndex = GUILayout.SelectionGrid(_selectIndex, _windowsTitle, 6, GUILayout.Height(60));
            if (_currentWindow == null && _childWindows.Length > 0)
            {
                selectIndex = 0;
            }
            if (selectIndex != _selectIndex)
            {
                _currentWindow?.OnExit();
                _selectIndex = selectIndex;
                _currentWindow = _childWindows[_selectIndex];
                _currentWindow.OnEnter();
            }
            if (_currentWindow != null)
            {
                _currentWindow.OnDraw();
            }
        }
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

        /// <summary>
        /// 自动调用
        /// </summary>
        public void AutoTakeSample()
        {
			foreach (var item in _childWindows)
			{
                var mi = item.GetType().GetMethod("TakeSample", BindingFlags.Instance | BindingFlags.NonPublic);
                if (mi != null)
                {
                    mi.Invoke(item,null);
                }
            }
        }
    
    }

    //内存分析基类
    internal abstract class MemoryProfilerBase<T> : IDebuggerWindow where T : UnityEngine.Object
    {
        Vector2 _scrollPos = Vector2.zero;
        protected List<ProfilerSample> _samples = new List<ProfilerSample>();
        protected long _sampleSize = 0;
        protected DateTime _sampleDateTime = DateTime.MinValue;

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
            string typeName = typeof(T).Name;
            GUILayout.Label(string.Format("<b>{0} Runtime Memory Information</b>", typeName));
            if (GUILayout.Button("Take Sample"))
            {
                TakeSample();
            }
            OnScrollViewDraw();
            GUILayout.EndScrollView();
        }

        protected virtual void OnScrollViewDraw()
        {
            // if (m_SampleTime <= DateTime.MinValue)
            // {
            //     GUILayout.Label(Utility.Text.Format("<b>Please take sample for {0} first.</b>", typeName));
            // }
            // else
            {
                // if (m_DuplicateSimpleCount > 0)
                // {
                //     GUILayout.Label(Utility.Text.Format("<b>{0} {1}s ({2}) obtained at {3}, while {4} {1}s ({5}) might be duplicated.</b>", m_Samples.Count.ToString(), typeName, GetByteLengthString(m_SampleSize), m_SampleTime.ToString("yyyy-MM-dd HH:mm:ss"), m_DuplicateSimpleCount.ToString(), GetByteLengthString(m_DuplicateSampleSize)));
                // }
                // else
                // {
                //     GUILayout.Label(Utility.Text.Format("<b>{0} {1}s ({2}) obtained at {3}.</b>", m_Samples.Count.ToString(), typeName, GetByteLengthString(m_SampleSize), m_SampleTime.ToString("yyyy-MM-dd HH:mm:ss")));
                // }
                string typeName = typeof(T).Name;
                GUILayout.Label(string.Format("<b>{0} {1}s ({2}) obtained at {3}.</b>", _samples.Count.ToString(), typeName, _sampleSize.ToByteLengthString(), _sampleDateTime.ToString("yyyy-MM-dd HH:mm:ss")));
                if (_samples.Count > 0)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(string.Format("<b>{0} Name</b>", typeName));
                        GUILayout.Label("<b>Type</b>", GUILayout.Width(240f));
                        GUILayout.Label("<b>Size</b>", GUILayout.Width(80f));
                    }
                    GUILayout.EndHorizontal();
                }

                for (int i = 0; i < _samples.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(_samples[i].Highlight ? string.Format("<color=yellow>{0}</color>", _samples[i].Name) : _samples[i].Name);
                        GUILayout.Label(_samples[i].Highlight ? string.Format("<color=yellow>{0}</color>", _samples[i].TypeName) : _samples[i].TypeName, GUILayout.Width(240f));
                        GUILayout.Label(_samples[i].Highlight ? string.Format("<color=yellow>{0}</color>", _samples[i].Size.ToByteLengthString()) : _samples[i].Size.ToByteLengthString(), GUILayout.Width(80f));
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        protected virtual void TakeSample()
        {
            ReleaseSamples();

            _sampleSize = 0;
            _sampleDateTime = DateTime.Now;
            //整理所有的数据
            T[] samples = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < samples.Length; i++)
            {
                long sampleSize = Profiler.GetRuntimeMemorySizeLong(samples[i]);
                var sample = ProfilerSamplePool.Get(samples[i].name, samples[i].GetType().Name, sampleSize);
                _samples.Add(sample);

                _sampleSize += sampleSize;
            }
            //sort
            _samples = _samples.OrderByDescending(x => x.Size).ToList();

            //将性能信息整理到日志中
            if (_samples.Count > 0)
            {
                //整体性能日志
                SummaryProfiler.ProfilerToString();

                Log.StrBuilder.Clear();
                Log.StrBuilder.AppendLine($"[*] [MemoryProfiler] [{typeof(T).FullName}]");
                Log.StrBuilder.AppendLine("{");
                foreach (var item in _samples)
                {
                    Log.StrBuilder.AppendLine(item.ToString());
                }
                Log.StrBuilder.AppendLine("}");
                Log.Info(Log.StrBuilder.ToString());
            }
        }

        //释放所有的数据
        protected void ReleaseSamples()
        {
            foreach (var item in _samples)
            {
                ProfilerSamplePool.Release(item);
            }
            _samples.Clear();
        }
    }
    //概要内存分析
    internal class SummaryMemoryProfiler : MemoryProfilerBase<UnityEngine.Object>
    {

        protected override void TakeSample()
        {
            base.TakeSample();
            Dictionary<string, ProfilerSample> newSamples = new Dictionary<string, ProfilerSample>();
            ProfilerSample tempSample;
            for (int i = 0; i < _samples.Count; i++)
            {
                tempSample = _samples[i];
                ProfilerSample sample;
                if (!newSamples.TryGetValue(tempSample.TypeName, out sample))
                {
                    sample = ProfilerSamplePool.Get(tempSample.TypeName, tempSample.TypeName, tempSample.Size, tempSample.Highlight);
                    newSamples.Add(tempSample.TypeName, sample);
                }
                else
                {
                    sample.Size += tempSample.Size;
                }
                ProfilerSamplePool.Release(tempSample);
                tempSample = null;
            }
            _samples.Clear();
            foreach (var item in newSamples.Values)
            {
                _samples.Add(item);
            }
            newSamples.Clear();
        }
    }
    //所有的内存分析
    internal class AllMemoryProfiler : MemoryProfilerBase<UnityEngine.Object>
    {
        protected override void TakeSample()
        {
            base.TakeSample();
            for (int i = 0; i < _samples.Count; i++)
            {
                if (_samples[i].Size < 1024)
                {
                    for (int j = i; j < _samples.Count; j++)
                    {
                        ProfilerSamplePool.Release(_samples[j]);
                    }
                    _samples.RemoveRange(i, _samples.Count - i);
                    break;
                }
            }
        }
    }
    //Texture内存分析
    internal class TextureMemoryProfiler : MemoryProfilerBase<Texture>
    {
    }
    //Mesh内存分析
    internal class MeshMemoryProfiler : MemoryProfilerBase<Mesh>
    {

    }
    //Material内存分析
    internal class MaterialMemoryProfiler : MemoryProfilerBase<Material>
    {

    }
    //Shader内存分析
    internal class ShaderMemoryProfiler : MemoryProfilerBase<Shader>
    {

    }
    //AnimationClip内存分析
    internal class AnimationClipMemoryProfiler : MemoryProfilerBase<AnimationClip>
    {

    }
    //AudioClip内存分析
    internal class AudioClipMemoryProfiler : MemoryProfilerBase<AudioClip>
    {

    }
    // Font内存分析
    internal class FontMemoryProfiler : MemoryProfilerBase<Font>
    {

    }
    //TextAsset内存分析
    internal class TextAssetMemoryProfiler : MemoryProfilerBase<TextAsset>
    {

    }
    //ScriptableObject内存分析
    internal class ScriptableObjectMemoryProfiler : MemoryProfilerBase<ScriptableObject>
    {

    }

    // profiler 
    internal class ProfilerSample
    {
        public string Name { get; private set; }
        public string TypeName { get; private set; }
        public long Size { get; set; }
        public bool Highlight { get; set; }

        public ProfilerSample Set(string name, string type, long size, bool highlight)
        {
            Name = name;
            TypeName = type;
            Size = size;
            Highlight = highlight;
            return this;
        }

		public override string ToString()
		{
            return $"   >   {TypeName} {Highlight} {Size.ToByteLengthString()} {Name}";
		}
	}
    // profiler pool
    internal class ProfilerSamplePool
    {
        static ObjectPool<ProfilerSample> _pool = new ObjectPool<ProfilerSample>(null, null);

        public static ProfilerSample Get(string name, string type, long size, bool highlight = false)
        {
            return _pool.Get().Set(name, type, size, highlight);
        }

        public static void Release(ProfilerSample sample)
        {
            _pool.Release(sample);
        }

    }

}

