using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wanderer.GameFramework
{
    [DebuggerWindow("Information")]
    public class InformationWindow : ToolbarDebuggerWindow
    {
        public override void OnInit(params object[] args)
        {
            base.OnInit(args);

            var windows = new IDebuggerWindow[]{new SystemInformation(),new EnvironmentInformation()
            ,new ScreenInformation(),new GraphicsInformation(),new InputInformation(),new OtherInformation()};
            var windowTitle = new string[windows.Length];
            for (int i = 0; i < windowTitle.Length; i++)
            {
                windowTitle[i] = windows[i].GetType().Name.Replace("Information", "");
            }
            //设置子窗口
            SetChildWindows(windows, windowTitle, args);
        }

    }


    //系统信息
    internal class SystemInformation : IDebuggerWindow
    {
        Vector2 _scrollPos = Vector2.zero;
        public void OnClose()
        {
        }

        public void OnDraw()
        {
            GUILayout.Label("<b>System Information</b>");
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");
            {
                GuiUtility.DrawItem("Device Unique ID", SystemInfo.deviceUniqueIdentifier);
                GuiUtility.DrawItem("Device Name", SystemInfo.deviceName);
                GuiUtility.DrawItem("Device Type", SystemInfo.deviceType.ToString());
                GuiUtility.DrawItem("Device Model", SystemInfo.deviceModel);
                GuiUtility.DrawItem("Processor Type", SystemInfo.processorType);
                GuiUtility.DrawItem("Processor Count", SystemInfo.processorCount.ToString());
                GuiUtility.DrawItem("Processor Frequency", string.Format("{0} MHz", SystemInfo.processorFrequency.ToString()));
                GuiUtility.DrawItem("System Memory Size", string.Format("{0} MB", SystemInfo.systemMemorySize.ToString()));
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Operating System Family", SystemInfo.operatingSystemFamily.ToString());
#endif
                GuiUtility.DrawItem("Operating System", SystemInfo.operatingSystem);
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Battery Status", SystemInfo.batteryStatus.ToString());
                GuiUtility.DrawItem("Battery Level", SystemInfo.batteryLevel.ToString("f12"));
#endif
#if UNITY_5_4_OR_NEWER
                GuiUtility.DrawItem("Supports Audio", SystemInfo.supportsAudio.ToString());
#endif
                GuiUtility.DrawItem("Supports Location Service", SystemInfo.supportsLocationService.ToString());
                GuiUtility.DrawItem("Supports Accelerometer", SystemInfo.supportsAccelerometer.ToString());
                GuiUtility.DrawItem("Supports Gyroscope", SystemInfo.supportsGyroscope.ToString());
                GuiUtility.DrawItem("Supports Vibration", SystemInfo.supportsVibration.ToString());
                GuiUtility.DrawItem("Genuine", Application.genuine.ToString());
                GuiUtility.DrawItem("Genuine Check Available", Application.genuineCheckAvailable.ToString());
            }
            GUILayout.EndScrollView();
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnInit(params object[] args)
        {

        }


    }

    //环境信息
    internal class EnvironmentInformation : IDebuggerWindow
    {
        private Vector2 _scrollPos = Vector2.zero;
        public void OnClose()
        {
        }

        public void OnDraw()
        {
            GUILayout.Label("<b>Environment Information</b>");
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");
            {
                GuiUtility.DrawItem("Product Name", Application.productName);
                GuiUtility.DrawItem("Company Name", Application.companyName);
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Game Identifier", Application.identifier);
#else
                    GuiUtility.DrawItem("Game Identifier", Application.bundleIdentifier);
#endif
                //  GuiUtility.DrawItem("Game Framework Version", Version.GameFrameworkVersion);
                //    GuiUtility.DrawItem("Game Version", string.Format("{0} ({1})", Version.GameVersion, Version.InternalGameVersion.ToString()));
                //  GuiUtility.DrawItem("Resource Version", m_BaseComponent.EditorResourceMode ? "Unavailable in editor resource mode" : (string.IsNullOrEmpty(m_ResourceComponent.ApplicableGameVersion) ? "Unknown" : Utility.Text.Format("{0} ({1})", m_ResourceComponent.ApplicableGameVersion, m_ResourceComponent.InternalResourceVersion.ToString())));
                GuiUtility.DrawItem("Application Version", Application.version);
                GuiUtility.DrawItem("Unity Version", Application.unityVersion);
                GuiUtility.DrawItem("Platform", Application.platform.ToString());
                GuiUtility.DrawItem("System Language", Application.systemLanguage.ToString());
                GuiUtility.DrawItem("Cloud Project Id", Application.cloudProjectId);
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Build Guid", Application.buildGUID);
#endif
                GuiUtility.DrawItem("Target Frame Rate", Application.targetFrameRate.ToString());
                GuiUtility.DrawItem("Internet Reachability", Application.internetReachability.ToString());
                GuiUtility.DrawItem("Background Loading Priority", Application.backgroundLoadingPriority.ToString());
                GuiUtility.DrawItem("Is Playing", Application.isPlaying.ToString());
#if UNITY_5_5_OR_NEWER
                //    GuiUtility.DrawItem("Splash Screen Is Finished", SplashScreen.isFinished.ToString());
#else
                    GuiUtility.DrawItem("Is Showing Splash Screen", Application.isShowingSplashScreen.ToString());
#endif
                GuiUtility.DrawItem("Run In Background", Application.runInBackground.ToString());
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Install Name", Application.installerName);
#endif
                GuiUtility.DrawItem("Install Mode", Application.installMode.ToString());
                GuiUtility.DrawItem("Sandbox Type", Application.sandboxType.ToString());
                GuiUtility.DrawItem("Is Mobile Platform", Application.isMobilePlatform.ToString());
                GuiUtility.DrawItem("Is Console Platform", Application.isConsolePlatform.ToString());
                GuiUtility.DrawItem("Is Editor", Application.isEditor.ToString());
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Is Focused", Application.isFocused.ToString());
#endif
#if UNITY_2018_2_OR_NEWER
                GuiUtility.DrawItem("Is Batch Mode", Application.isBatchMode.ToString());
#endif
#if UNITY_5_3
                    GuiUtility.DrawItem("Stack Trace Log Type", Application.stackTraceLogType.ToString());
#endif
            }
            GUILayout.EndScrollView();
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnInit(params object[] args)
        {
        }
    }

    //屏幕信息
    internal class ScreenInformation : IDebuggerWindow
    {
        Vector2 _scrollPos = Vector2.zero;
        public void OnClose()
        {
        }

        public void OnDraw()
        {
            GUILayout.Label("<b>Scene Information</b>");
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");
            {
                GuiUtility.DrawItem("Scene Count", SceneManager.sceneCount.ToString());
                GuiUtility.DrawItem("Scene Count In Build Settings", SceneManager.sceneCountInBuildSettings.ToString());

                Scene activeScene = SceneManager.GetActiveScene();
                GuiUtility.DrawItem("Active Scene Name", activeScene.name);
                GuiUtility.DrawItem("Active Scene Path", activeScene.path);
                GuiUtility.DrawItem("Active Scene Build Index", activeScene.buildIndex.ToString());
                GuiUtility.DrawItem("Active Scene Is Dirty", activeScene.isDirty.ToString());
                GuiUtility.DrawItem("Active Scene Is Loaded", activeScene.isLoaded.ToString());
                GuiUtility.DrawItem("Active Scene Is Valid", activeScene.IsValid().ToString());
                GuiUtility.DrawItem("Active Scene Root Count", activeScene.rootCount.ToString());
            }
            GUILayout.EndScrollView();
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnInit(params object[] args)
        {
        }
    }

    //图像信息
    internal class GraphicsInformation : IDebuggerWindow
    {
        Vector2 _scrollPos = Vector2.zero;
        public void OnClose()
        {
        }

        public void OnDraw()
        {
            GUILayout.Label("<b>Graphics Information</b>");
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");
            {
                GuiUtility.DrawItem("Device ID", SystemInfo.graphicsDeviceID.ToString());
                GuiUtility.DrawItem("Device Name", SystemInfo.graphicsDeviceName);
                GuiUtility.DrawItem("Device Vendor ID", SystemInfo.graphicsDeviceVendorID.ToString());
                GuiUtility.DrawItem("Device Vendor", SystemInfo.graphicsDeviceVendor);
                GuiUtility.DrawItem("Device Type", SystemInfo.graphicsDeviceType.ToString());
                GuiUtility.DrawItem("Device Version", SystemInfo.graphicsDeviceVersion);
                GuiUtility.DrawItem("Memory Size", string.Format("{0} MB", SystemInfo.graphicsMemorySize.ToString()));
                GuiUtility.DrawItem("Multi Threaded", SystemInfo.graphicsMultiThreaded.ToString());
                GuiUtility.DrawItem("Shader Level", SystemInfo.graphicsShaderLevel.ToString());
                GuiUtility.DrawItem("Global Maximum LOD", Shader.globalMaximumLOD.ToString());
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Global Render Pipeline", Shader.globalRenderPipeline);
#endif
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Active Tier", Graphics.activeTier.ToString());
#endif
#if UNITY_2017_2_OR_NEWER
                GuiUtility.DrawItem("Active Color Gamut", Graphics.activeColorGamut.ToString());
#endif
#if UNITY_2019_2_OR_NEWER
                GuiUtility.DrawItem("Preserve Frame Buffer Alpha", Graphics.preserveFramebufferAlpha.ToString());
#endif
                GuiUtility.DrawItem("NPOT Support", SystemInfo.npotSupport.ToString());
                GuiUtility.DrawItem("Max Texture Size", SystemInfo.maxTextureSize.ToString());
                GuiUtility.DrawItem("Supported Render Target Count", SystemInfo.supportedRenderTargetCount.ToString());
#if UNITY_5_4_OR_NEWER
                GuiUtility.DrawItem("Copy Texture Support", SystemInfo.copyTextureSupport.ToString());
#endif
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Uses Reversed ZBuffer", SystemInfo.usesReversedZBuffer.ToString());
#endif
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Max Cubemap Size", SystemInfo.maxCubemapSize.ToString());
                GuiUtility.DrawItem("Graphics UV Starts At Top", SystemInfo.graphicsUVStartsAtTop.ToString());
#endif
#if UNITY_2019_1_OR_NEWER
                GuiUtility.DrawItem("Min Constant Buffer Offset Alignment", SystemInfo.minConstantBufferOffsetAlignment.ToString());
#endif
#if UNITY_2018_3_OR_NEWER
                GuiUtility.DrawItem("Has Hidden Surface Removal On GPU", SystemInfo.hasHiddenSurfaceRemovalOnGPU.ToString());
                GuiUtility.DrawItem("Has Dynamic Uniform Array Indexing In Fragment Shaders", SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders.ToString());
#endif
#if UNITY_2019_2_OR_NEWER
                GuiUtility.DrawItem("Has Mip Max Level", SystemInfo.hasMipMaxLevel.ToString());
#endif
#if UNITY_5_3 || UNITY_5_4
                    GuiUtility.DrawItem("Supports Stencil", SystemInfo.supportsStencil.ToString());
                    GuiUtility.DrawItem("Supports Render Textures", SystemInfo.supportsRenderTextures.ToString());
#endif
                GuiUtility.DrawItem("Supports Sparse Textures", SystemInfo.supportsSparseTextures.ToString());
                GuiUtility.DrawItem("Supports 3D Textures", SystemInfo.supports3DTextures.ToString());
                GuiUtility.DrawItem("Supports Shadows", SystemInfo.supportsShadows.ToString());
                GuiUtility.DrawItem("Supports Raw Shadow Depth Sampling", SystemInfo.supportsRawShadowDepthSampling.ToString());
#if !UNITY_2019_1_OR_NEWER
                    GuiUtility.DrawItem("Supports Render To Cubemap", SystemInfo.supportsRenderToCubemap.ToString());
#endif
                GuiUtility.DrawItem("Supports Compute Shader", SystemInfo.supportsComputeShaders.ToString());
                GuiUtility.DrawItem("Supports Instancing", SystemInfo.supportsInstancing.ToString());
#if !UNITY_2019_1_OR_NEWER
                    GuiUtility.DrawItem("Supports Image Effects", SystemInfo.supportsImageEffects.ToString());
#endif
#if UNITY_5_4_OR_NEWER
                GuiUtility.DrawItem("Supports 2D Array Textures", SystemInfo.supports2DArrayTextures.ToString());
                GuiUtility.DrawItem("Supports Motion Vectors", SystemInfo.supportsMotionVectors.ToString());
#endif
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Supports Cubemap Array Textures", SystemInfo.supportsCubemapArrayTextures.ToString());
#endif
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Supports 3D Render Textures", SystemInfo.supports3DRenderTextures.ToString());
#endif
#if UNITY_2017_2_OR_NEWER && !UNITY_2017_2_0 || UNITY_2017_1_4
                GuiUtility.DrawItem("Supports Texture Wrap Mirror Once", SystemInfo.supportsTextureWrapMirrorOnce.ToString());
#endif
#if UNITY_2019_1_OR_NEWER
                GuiUtility.DrawItem("Supports Graphics Fence", SystemInfo.supportsGraphicsFence.ToString());
#elif UNITY_2017_3_OR_NEWER
                    GuiUtility.DrawItem("Supports GPU Fence", SystemInfo.supportsGPUFence.ToString());
#endif
#if UNITY_2017_3_OR_NEWER
                GuiUtility.DrawItem("Supports Async Compute", SystemInfo.supportsAsyncCompute.ToString());
                GuiUtility.DrawItem("Supports Multisampled Textures", SystemInfo.supportsMultisampledTextures.ToString());
#endif
#if UNITY_2018_1_OR_NEWER
                GuiUtility.DrawItem("Supports Async GPU Readback", SystemInfo.supportsAsyncGPUReadback.ToString());
                GuiUtility.DrawItem("Supports 32bits Index Buffer", SystemInfo.supports32bitsIndexBuffer.ToString());
                GuiUtility.DrawItem("Supports Hardware Quad Topology", SystemInfo.supportsHardwareQuadTopology.ToString());
#endif
#if UNITY_2018_2_OR_NEWER
                GuiUtility.DrawItem("Supports Mip Streaming", SystemInfo.supportsMipStreaming.ToString());
                GuiUtility.DrawItem("Supports Multisample Auto Resolve", SystemInfo.supportsMultisampleAutoResolve.ToString());
#endif
#if UNITY_2018_3_OR_NEWER
                GuiUtility.DrawItem("Supports Separated Render Targets Blend", SystemInfo.supportsSeparatedRenderTargetsBlend.ToString());
#endif
#if UNITY_2019_1_OR_NEWER
                GuiUtility.DrawItem("Supports Set Constant Buffer", SystemInfo.supportsSetConstantBuffer.ToString());
#endif
            }
            GUILayout.EndScrollView();
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnInit(params object[] args)
        {
        }
    }

    //输入信息
    internal class InputInformation : ToolbarDebuggerWindow
    {
        public override void OnInit(params object[] args)
        {
            base.OnInit(args);

            var windows = new IDebuggerWindow[]{new SummaryInputInformation(),new TouchInputInformation()
            ,new LocationInputInformation(),new AccelerationInputInformation(),new GyroscopeInputInformation(),new CompassInputInformation()};
            var windowTitle = new string[windows.Length];
            for (int i = 0; i < windowTitle.Length; i++)
            {
                windowTitle[i] = windows[i].GetType().Name.Replace("InputInformation", "");
            }
            //设置子窗口
            SetChildWindows(windows, windowTitle, args);
        }
    }

    internal class SummaryInputInformation : DebuggerWindowBase
    {
    }

    internal class TouchInputInformation : DebuggerWindowBase
    {
    }

    internal class LocationInputInformation : DebuggerWindowBase
    {
    }

    internal class AccelerationInputInformation : DebuggerWindowBase
    {
    }

    internal class GyroscopeInputInformation : DebuggerWindowBase
    {
    }

    internal class CompassInputInformation : DebuggerWindowBase
    {
    }

    //其他信息
    internal class OtherInformation : ToolbarDebuggerWindow
    {
        public override void OnInit(params object[] args)
        {
            base.OnInit(args);

            var windows = new IDebuggerWindow[]{new ScreenOtherInformation(),new PathOtherInformation()
            ,new TimeOtherInformation(),new QualityOtherInformation(),new WebPlayerOtherInformation()};
            var windowTitle = new string[windows.Length];
            for (int i = 0; i < windowTitle.Length; i++)
            {
                windowTitle[i] = windows[i].GetType().Name.Replace("OtherInformation", "");
            }
            //设置子窗口
            SetChildWindows(windows, windowTitle, args);
        }
    }

    internal class ScreenOtherInformation : DebuggerWindowBase
    {
    }
    internal class PathOtherInformation : DebuggerWindowBase
    {
    }
    internal class TimeOtherInformation : DebuggerWindowBase
    {
    }
    internal class QualityOtherInformation : DebuggerWindowBase
    {
    }
    internal class WebPlayerOtherInformation : DebuggerWindowBase
    {
    }


}
