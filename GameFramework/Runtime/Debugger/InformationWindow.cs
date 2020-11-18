using System.Collections;
using System.Collections.Generic;
using System.Text;
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


        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("[INFORMATION START]");
            foreach (var item in _childWindows)
			{
                strBuilder.AppendLine($"{item.GetType().Name}{{{item.ToString()}}}");
            }
            strBuilder.AppendLine("[INFORMATION END]");
            return strBuilder.ToString();
        }

    }


    //系统信息
    internal class SystemInformation : IDebuggerWindow
    {
        //信息文本
        private string _infoText;

        Vector2 _scrollPos = Vector2.zero;
        public void OnClose()
        {
        }

        public void OnDraw()
        {
            if (string.IsNullOrEmpty(_infoText))
            {
                GuiUtility.RecordTextStart();
            }

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


            if (string.IsNullOrEmpty(_infoText))
            {
                _infoText=GuiUtility.RecordTextStop();
            }
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

		public override string ToString()
		{
            return _infoText;
        }

	}

    //环境信息
    internal class EnvironmentInformation : IDebuggerWindow
    {
        string _infoText;
        private Vector2 _scrollPos = Vector2.zero;
        public void OnClose()
        {
        }

        public void OnDraw()
        {
            if (string.IsNullOrEmpty(_infoText))
            {
                GuiUtility.RecordTextStart();
            }

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

            if (string.IsNullOrEmpty(_infoText))
            {
                _infoText = GuiUtility.RecordTextStop();
            }
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


		public override string ToString()
		{
            return _infoText;
		}

	}

    //屏幕信息
    internal class ScreenInformation : IDebuggerWindow
    {
        string _infoText;

        Vector2 _scrollPos = Vector2.zero;
        public void OnClose()
        {
        }

        public void OnDraw()
        {
            if (string.IsNullOrEmpty(_infoText))
            {
                GuiUtility.RecordTextStart();
            }

            GUILayout.Label("<b>Scene Information</b>");
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");
            {
                GuiUtility.DrawItem("Current Resolution", GetResolutionString(Screen.currentResolution));
                GuiUtility.DrawItem("Screen Width", string.Format("{0} px / {1} in / {2} cm", Screen.width.ToString(), GetInchesFromPixels(Screen.width).ToString("F2"), GetCentimetersFromPixels(Screen.width).ToString("F2")));
                GuiUtility.DrawItem("Screen Height", string.Format("{0} px / {1} in / {2} cm", Screen.height.ToString(), GetInchesFromPixels(Screen.height).ToString("F2"), GetCentimetersFromPixels(Screen.height).ToString("F2")));
                GuiUtility.DrawItem("Screen DPI", Screen.dpi.ToString("F2"));
                GuiUtility.DrawItem("Screen Orientation", Screen.orientation.ToString());
                GuiUtility.DrawItem("Is Full Screen", Screen.fullScreen.ToString());
#if UNITY_2018_1_OR_NEWER
                GuiUtility.DrawItem("Full Screen Mode", Screen.fullScreenMode.ToString());
#endif
                GuiUtility.DrawItem("Sleep Timeout", GetSleepTimeoutDescription(Screen.sleepTimeout));
#if UNITY_2019_2_OR_NEWER
                GuiUtility.DrawItem("Brightness", Screen.brightness.ToString("F2"));
#endif
                GuiUtility.DrawItem("Cursor Visible", Cursor.visible.ToString());
                GuiUtility.DrawItem("Cursor Lock State", Cursor.lockState.ToString());
                GuiUtility.DrawItem("Auto Landscape Left", Screen.autorotateToLandscapeLeft.ToString());
                GuiUtility.DrawItem("Auto Landscape Right", Screen.autorotateToLandscapeRight.ToString());
                GuiUtility.DrawItem("Auto Portrait", Screen.autorotateToPortrait.ToString());
                GuiUtility.DrawItem("Auto Portrait Upside Down", Screen.autorotateToPortraitUpsideDown.ToString());
#if UNITY_2017_2_OR_NEWER && !UNITY_2017_2_0
                GuiUtility.DrawItem("Safe Area", Screen.safeArea.ToString());
#endif
#if UNITY_2019_2_OR_NEWER
                GuiUtility.DrawItem("Cutouts", GetCutoutsString(Screen.cutouts));
#endif
                GuiUtility.DrawItem("Support Resolutions", GetResolutionsString(Screen.resolutions));
            }
            GUILayout.EndScrollView();

            if (string.IsNullOrEmpty(_infoText))
            {
                _infoText = GuiUtility.RecordTextStop();
            }

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

		public override string ToString()
		{
            return _infoText;
        }

		private string GetSleepTimeoutDescription(int sleepTimeout)
        {
            if (sleepTimeout == SleepTimeout.NeverSleep)
            {
                return "Never Sleep";
            }

            if (sleepTimeout == SleepTimeout.SystemSetting)
            {
                return "System Setting";
            }

            return sleepTimeout.ToString();
        }

        private string GetResolutionString(Resolution resolution)
        {
            return string.Format("{0} x {1} @ {2}Hz", resolution.width.ToString(), resolution.height.ToString(), resolution.refreshRate.ToString());
        }

        private string GetCutoutsString(Rect[] cutouts)
        {
            string[] cutoutStrings = new string[cutouts.Length];
            for (int i = 0; i < cutouts.Length; i++)
            {
                cutoutStrings[i] = cutouts[i].ToString();
            }

            return string.Join("; ", cutoutStrings);
        }

        private string GetResolutionsString(Resolution[] resolutions)
        {
            string[] resolutionStrings = new string[resolutions.Length];
            for (int i = 0; i < resolutions.Length; i++)
            {
                resolutionStrings[i] = GetResolutionString(resolutions[i]);
            }

            return string.Join("; ", resolutionStrings);
        }

        /// <summary>
        /// 将像素转换为英寸。
        /// </summary>
        /// <param name="pixels">像素。</param>
        /// <returns>英寸。</returns>
        public float GetInchesFromPixels(float pixels)
        {
            return pixels / Screen.dpi;
        }

        /// <summary>
        /// 将像素转换为厘米。
        /// </summary>
        /// <param name="pixels">像素。</param>
        /// <returns>厘米。</returns>
        public static float GetCentimetersFromPixels(float pixels)
        {
            float inchesToCentimeters = 2.54f;  // 1 inch = 2.54 cm
            return inchesToCentimeters * pixels / Screen.dpi;
        }
    }

    //图像信息
    internal class GraphicsInformation : IDebuggerWindow
    {
        string _infoText;
        Vector2 _scrollPos = Vector2.zero;
        public void OnClose()
        {
        }

        public void OnDraw()
        {
            if (string.IsNullOrEmpty(_infoText))
            {
                 GuiUtility.RecordTextStart();
            }

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

            if (string.IsNullOrEmpty(_infoText))
            {
                _infoText = GuiUtility.RecordTextStop();
            }
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

		public override string ToString()
		{
            return _infoText;
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
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Input Summary Information</b>");
            GUILayout.BeginVertical("box");
            {
                GuiUtility.DrawItem("Back Button Leaves App", Input.backButtonLeavesApp.ToString());
                GuiUtility.DrawItem("Device Orientation", Input.deviceOrientation.ToString());
                GuiUtility.DrawItem("Mouse Present", Input.mousePresent.ToString());
                GuiUtility.DrawItem("Mouse Position", Input.mousePosition.ToString());
                GuiUtility.DrawItem("Mouse Scroll Delta", Input.mouseScrollDelta.ToString());
                GuiUtility.DrawItem("Any Key", Input.anyKey.ToString());
                GuiUtility.DrawItem("Any Key Down", Input.anyKeyDown.ToString());
                GuiUtility.DrawItem("Input String", Input.inputString);
                GuiUtility.DrawItem("IME Is Selected", Input.imeIsSelected.ToString());
                GuiUtility.DrawItem("IME Composition Mode", Input.imeCompositionMode.ToString());
                GuiUtility.DrawItem("Compensate Sensors", Input.compensateSensors.ToString());
                GuiUtility.DrawItem("Composition Cursor Position", Input.compositionCursorPos.ToString());
                GuiUtility.DrawItem("Composition String", Input.compositionString);
            }
            GUILayout.EndVertical();
        }
    }

    internal class TouchInputInformation : DebuggerWindowBase
    {
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Input Touch Information</b>");
            GUILayout.BeginVertical("box");
            {
                GuiUtility.DrawItem("Touch Supported", Input.touchSupported.ToString());
                GuiUtility.DrawItem("Touch Pressure Supported", Input.touchPressureSupported.ToString());
                GuiUtility.DrawItem("Stylus Touch Supported", Input.stylusTouchSupported.ToString());
                GuiUtility.DrawItem("Simulate Mouse With Touches", Input.simulateMouseWithTouches.ToString());
                GuiUtility.DrawItem("Multi Touch Enabled", Input.multiTouchEnabled.ToString());
                GuiUtility.DrawItem("Touch Count", Input.touchCount.ToString());
                GuiUtility.DrawItem("Touches", GetTouchesString(Input.touches));
            }
            GUILayout.EndVertical();
        }

        private string GetTouchString(Touch touch)
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}", touch.position.ToString(), touch.deltaPosition.ToString(), touch.rawPosition.ToString(), touch.pressure.ToString(), touch.phase.ToString());
        }

        private string GetTouchesString(Touch[] touches)
        {
            string[] touchStrings = new string[touches.Length];
            for (int i = 0; i < touches.Length; i++)
            {
                touchStrings[i] = GetTouchString(touches[i]);
            }

            return string.Join("; ", touchStrings);
        }
    }

    internal class LocationInputInformation : DebuggerWindowBase
    {
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Input Location Information</b>");
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                    {
                        Input.location.Start();
                    }
                    if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                    {
                        Input.location.Stop();
                    }
                }
                GUILayout.EndHorizontal();

                GuiUtility.DrawItem("Is Enabled By User", Input.location.isEnabledByUser.ToString());
                GuiUtility.DrawItem("Status", Input.location.status.ToString());
                if (Input.location.status == LocationServiceStatus.Running)
                {
                    GuiUtility.DrawItem("Horizontal Accuracy", Input.location.lastData.horizontalAccuracy.ToString());
                    GuiUtility.DrawItem("Vertical Accuracy", Input.location.lastData.verticalAccuracy.ToString());
                    GuiUtility.DrawItem("Longitude", Input.location.lastData.longitude.ToString());
                    GuiUtility.DrawItem("Latitude", Input.location.lastData.latitude.ToString());
                    GuiUtility.DrawItem("Altitude", Input.location.lastData.altitude.ToString());
                    GuiUtility.DrawItem("Timestamp", Input.location.lastData.timestamp.ToString());
                }
            }
            GUILayout.EndVertical();
        }
    }

    internal class AccelerationInputInformation : DebuggerWindowBase
    {
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Input Acceleration Information</b>");
            GUILayout.BeginVertical("box");
            {
                GuiUtility.DrawItem("Acceleration", Input.acceleration.ToString());
                GuiUtility.DrawItem("Acceleration Event Count", Input.accelerationEventCount.ToString());
                GuiUtility.DrawItem("Acceleration Events", GetAccelerationEventsString(Input.accelerationEvents));
            }
            GUILayout.EndVertical();
        }

        private string GetAccelerationEventString(AccelerationEvent accelerationEvent)
        {
            return string.Format("{0}, {1}", accelerationEvent.acceleration.ToString(), accelerationEvent.deltaTime.ToString());
        }

        private string GetAccelerationEventsString(AccelerationEvent[] accelerationEvents)
        {
            string[] accelerationEventStrings = new string[accelerationEvents.Length];
            for (int i = 0; i < accelerationEvents.Length; i++)
            {
                accelerationEventStrings[i] = GetAccelerationEventString(accelerationEvents[i]);
            }

            return string.Join("; ", accelerationEventStrings);
        }
    }

    internal class GyroscopeInputInformation : DebuggerWindowBase
    {
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Input Gyroscope Information</b>");
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                    {
                        Input.gyro.enabled = true;
                    }
                    if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                    {
                        Input.gyro.enabled = false;
                    }
                }
                GUILayout.EndHorizontal();

                GuiUtility.DrawItem("Enabled", Input.gyro.enabled.ToString());
                if (Input.gyro.enabled)
                {
                    GuiUtility.DrawItem("Update Interval", Input.gyro.updateInterval.ToString());
                    GuiUtility.DrawItem("Attitude", Input.gyro.attitude.eulerAngles.ToString());
                    GuiUtility.DrawItem("Gravity", Input.gyro.gravity.ToString());
                    GuiUtility.DrawItem("Rotation Rate", Input.gyro.rotationRate.ToString());
                    GuiUtility.DrawItem("Rotation Rate Unbiased", Input.gyro.rotationRateUnbiased.ToString());
                    GuiUtility.DrawItem("User Acceleration", Input.gyro.userAcceleration.ToString());
                }
            }
            GUILayout.EndVertical();
        }
    }

    internal class CompassInputInformation : DebuggerWindowBase
    {
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Input Compass Information</b>");
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                    {
                        Input.compass.enabled = true;
                    }
                    if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                    {
                        Input.compass.enabled = false;
                    }
                }
                GUILayout.EndHorizontal();

                GuiUtility.DrawItem("Enabled", Input.compass.enabled.ToString());
                if (Input.compass.enabled)
                {
                    GuiUtility.DrawItem("Heading Accuracy", Input.compass.headingAccuracy.ToString());
                    GuiUtility.DrawItem("Magnetic Heading", Input.compass.magneticHeading.ToString());
                    GuiUtility.DrawItem("Raw Vector", Input.compass.rawVector.ToString());
                    GuiUtility.DrawItem("Timestamp", Input.compass.timestamp.ToString());
                    GuiUtility.DrawItem("True Heading", Input.compass.trueHeading.ToString());
                }
            }
            GUILayout.EndVertical();
        }
    }

    //其他信息
    internal class OtherInformation : ToolbarDebuggerWindow
    {
        public override void OnInit(params object[] args)
        {
            base.OnInit(args);

            var windows = new IDebuggerWindow[]{new SceneOtherInformation(),new PathOtherInformation()
            ,new TimeOtherInformation(),new QualityOtherInformation()};
            var windowTitle = new string[windows.Length];
            for (int i = 0; i < windowTitle.Length; i++)
            {
                windowTitle[i] = windows[i].GetType().Name.Replace("OtherInformation", "");
            }
            //设置子窗口
            SetChildWindows(windows, windowTitle, args);
        }
    }

    //场景信息
    internal class SceneOtherInformation : DebuggerWindowBase
    {
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Scene Information</b>");
            GUILayout.BeginVertical("box");
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
            GUILayout.EndVertical();
        }
    }
    internal class PathOtherInformation : DebuggerWindowBase
    {
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Path Information</b>");
            GUILayout.BeginVertical("box");
            {
                GuiUtility.DrawItem("Data Path", Application.dataPath);
                GuiUtility.DrawItem("Persistent Data Path", Application.persistentDataPath);
                GuiUtility.DrawItem("Streaming Assets Path", Application.streamingAssetsPath);
                GuiUtility.DrawItem("Temporary Cache Path", Application.temporaryCachePath);
#if UNITY_2018_3_OR_NEWER
                GuiUtility.DrawItem("Console Log Path", Application.consoleLogPath);
#endif
            }
            GUILayout.EndVertical();
        }
    }
    internal class TimeOtherInformation : DebuggerWindowBase
    {
        private Vector2 _scrollPos = Vector2.zero;
        public override void OnDraw()
        {
            base.OnDraw();
            GUILayout.Label("<b>Time Information</b>");
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");
            {
                GuiUtility.DrawItem("Time Scale", string.Format("{0} [{1}]", Time.timeScale.ToString(), GetTimeScaleDescription(Time.timeScale)));
                GuiUtility.DrawItem("Realtime Since Startup", Time.realtimeSinceStartup.ToString());
                GuiUtility.DrawItem("Time Since Level Load", Time.timeSinceLevelLoad.ToString());
                GuiUtility.DrawItem("Time", Time.time.ToString());
                GuiUtility.DrawItem("Fixed Time", Time.fixedTime.ToString());
                GuiUtility.DrawItem("Unscaled Time", Time.unscaledTime.ToString());
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Fixed Unscaled Time", Time.fixedUnscaledTime.ToString());
#endif
                GuiUtility.DrawItem("Delta Time", Time.deltaTime.ToString());
                GuiUtility.DrawItem("Fixed Delta Time", Time.fixedDeltaTime.ToString());
                GuiUtility.DrawItem("Unscaled Delta Time", Time.unscaledDeltaTime.ToString());
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("Fixed Unscaled Delta Time", Time.fixedUnscaledDeltaTime.ToString());
#endif
                GuiUtility.DrawItem("Smooth Delta Time", Time.smoothDeltaTime.ToString());
                GuiUtility.DrawItem("Maximum Delta Time", Time.maximumDeltaTime.ToString());
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Maximum Particle Delta Time", Time.maximumParticleDeltaTime.ToString());
#endif
                GuiUtility.DrawItem("Frame Count", Time.frameCount.ToString());
                GuiUtility.DrawItem("Rendered Frame Count", Time.renderedFrameCount.ToString());
                GuiUtility.DrawItem("Capture Framerate", Time.captureFramerate.ToString());
#if UNITY_2019_2_OR_NEWER
                GuiUtility.DrawItem("Capture Delta Time", Time.captureDeltaTime.ToString());
#endif
#if UNITY_5_6_OR_NEWER
                GuiUtility.DrawItem("In Fixed Time Step", Time.inFixedTimeStep.ToString());
#endif
            }
            GUILayout.EndScrollView();
        }

        private string GetTimeScaleDescription(float timeScale)
        {
            if (timeScale <= 0f)
            {
                return "Pause";
            }

            if (timeScale < 1f)
            {
                return "Slower";
            }

            if (timeScale > 1f)
            {
                return "Faster";
            }

            return "Normal";
        }
    }


    internal class QualityOtherInformation : DebuggerWindowBase
    {
        private bool _applyExpensiveChanges = false;
        private Vector2 _scrollPos = Vector2.zero;

        public override void OnDraw()
        {
            base.OnDraw();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box");

            GUILayout.Label("<b>Quality Level</b>");
            GUILayout.BeginVertical("box");
            {
                int currentQualityLevel = QualitySettings.GetQualityLevel();

                GuiUtility.DrawItem("Current Quality Level", QualitySettings.names[currentQualityLevel]);
                _applyExpensiveChanges = GUILayout.Toggle(_applyExpensiveChanges, "Apply expensive changes on quality level change.");

                int newQualityLevel = GUILayout.SelectionGrid(currentQualityLevel, QualitySettings.names, 3, "toggle");
                if (newQualityLevel != currentQualityLevel)
                {
                    QualitySettings.SetQualityLevel(newQualityLevel, _applyExpensiveChanges);
                }
            }
            GUILayout.EndVertical();

            GUILayout.Label("<b>Rendering Information</b>");
            GUILayout.BeginVertical("box");
            {
                GuiUtility.DrawItem("Active Color Space", QualitySettings.activeColorSpace.ToString());
                GuiUtility.DrawItem("Desired Color Space", QualitySettings.desiredColorSpace.ToString());
                GuiUtility.DrawItem("Max Queued Frames", QualitySettings.maxQueuedFrames.ToString());
                GuiUtility.DrawItem("Pixel Light Count", QualitySettings.pixelLightCount.ToString());
                GuiUtility.DrawItem("Master Texture Limit", QualitySettings.masterTextureLimit.ToString());
                GuiUtility.DrawItem("Anisotropic Filtering", QualitySettings.anisotropicFiltering.ToString());
                GuiUtility.DrawItem("Anti Aliasing", QualitySettings.antiAliasing.ToString());
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Soft Particles", QualitySettings.softParticles.ToString());
#endif
                GuiUtility.DrawItem("Soft Vegetation", QualitySettings.softVegetation.ToString());
                GuiUtility.DrawItem("Realtime Reflection Probes", QualitySettings.realtimeReflectionProbes.ToString());
                GuiUtility.DrawItem("Billboards Face Camera Position", QualitySettings.billboardsFaceCameraPosition.ToString());
#if UNITY_2017_1_OR_NEWER
                GuiUtility.DrawItem("Resolution Scaling Fixed DPI Factor", QualitySettings.resolutionScalingFixedDPIFactor.ToString());
#endif
#if UNITY_2018_2_OR_NEWER
                GuiUtility.DrawItem("Texture Streaming Enabled", QualitySettings.streamingMipmapsActive.ToString());
                GuiUtility.DrawItem("Texture Streaming Add All Cameras", QualitySettings.streamingMipmapsAddAllCameras.ToString());
                GuiUtility.DrawItem("Texture Streaming Memory Budget", QualitySettings.streamingMipmapsMemoryBudget.ToString());
                GuiUtility.DrawItem("Texture Streaming Renderers Per Frame", QualitySettings.streamingMipmapsRenderersPerFrame.ToString());
                GuiUtility.DrawItem("Texture Streaming Max Level Reduction", QualitySettings.streamingMipmapsMaxLevelReduction.ToString());
                GuiUtility.DrawItem("Texture Streaming Max File IO Requests", QualitySettings.streamingMipmapsMaxFileIORequests.ToString());
#endif
            }
            GUILayout.EndVertical();

            GUILayout.Label("<b>Shadows Information</b>");
            GUILayout.BeginVertical("box");
            {
#if UNITY_2017_1_OR_NEWER
                GuiUtility.DrawItem("Shadowmask Mode", QualitySettings.shadowmaskMode.ToString());
#endif
#if UNITY_5_5_OR_NEWER
                GuiUtility.DrawItem("Shadow Quality", QualitySettings.shadows.ToString());
#endif
#if UNITY_5_4_OR_NEWER
                GuiUtility.DrawItem("Shadow Resolution", QualitySettings.shadowResolution.ToString());
#endif
                GuiUtility.DrawItem("Shadow Projection", QualitySettings.shadowProjection.ToString());
                GuiUtility.DrawItem("Shadow Distance", QualitySettings.shadowDistance.ToString());
                GuiUtility.DrawItem("Shadow Near Plane Offset", QualitySettings.shadowNearPlaneOffset.ToString());
                GuiUtility.DrawItem("Shadow Cascades", QualitySettings.shadowCascades.ToString());
                GuiUtility.DrawItem("Shadow Cascade 2 Split", QualitySettings.shadowCascade2Split.ToString());
                GuiUtility.DrawItem("Shadow Cascade 4 Split", QualitySettings.shadowCascade4Split.ToString());
            }
            GUILayout.EndVertical();

            GUILayout.Label("<b>Other Information</b>");
            GUILayout.BeginVertical("box");
            {
#if UNITY_2019_1_OR_NEWER
                GuiUtility.DrawItem("Skin Weights", QualitySettings.skinWeights.ToString());
#else
                    GuiUtility.DrawItem("Blend Weights", QualitySettings.blendWeights.ToString());
#endif
                GuiUtility.DrawItem("VSync Count", QualitySettings.vSyncCount.ToString());
                GuiUtility.DrawItem("LOD Bias", QualitySettings.lodBias.ToString());
                GuiUtility.DrawItem("Maximum LOD Level", QualitySettings.maximumLODLevel.ToString());
                GuiUtility.DrawItem("Particle Raycast Budget", QualitySettings.particleRaycastBudget.ToString());
                GuiUtility.DrawItem("Async Upload Time Slice", string.Format("{0} ms", QualitySettings.asyncUploadTimeSlice.ToString()));
                GuiUtility.DrawItem("Async Upload Buffer Size", string.Format("{0} MB", QualitySettings.asyncUploadBufferSize.ToString()));
#if UNITY_2018_3_OR_NEWER
                GuiUtility.DrawItem("Async Upload Persistent Buffer", QualitySettings.asyncUploadPersistentBuffer.ToString());
#endif
            }
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }
    }
    // internal class WebPlayerOtherInformation : DebuggerWindowBase
    // {
    //   
    // }


}
