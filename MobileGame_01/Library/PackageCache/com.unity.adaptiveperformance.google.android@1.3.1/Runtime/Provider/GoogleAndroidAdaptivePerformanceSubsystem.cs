#if UNITY_ANDROID

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine.AdaptivePerformance.Provider;
using AOT;
using UnityEngine.Rendering;

#if UNITY_2023_1_OR_NEWER
using UnityEngine.Android;
#endif

namespace UnityEngine.AdaptivePerformance.Google.Android
{
    internal static class ADPFLog
    {
        static GoogleAndroidProviderSettings settings = GoogleAndroidProviderSettings.GetSettings();

        [Conditional("DEVELOPMENT_BUILD")]
        public static void Debug(string format, params object[] args)
        {
            if (settings != null && settings.googleProviderLogging)
                UnityEngine.Debug.Log(System.String.Format("[AP ADPF] " + format, args));
        }
    }

    [Preserve]
    public class GoogleAndroidAdaptivePerformanceSubsystem : AdaptivePerformanceSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static AdaptivePerformanceSubsystemDescriptor RegisterDescriptor()
        {
            if (!GoogleAndroidAdaptivePerformanceSubsystemProvider.NativeApi.IsAvailable())
            {
                ADPFLog.Debug($"The native API for this provider is not available. Aborting registering the Adaptive Performance provider descriptor.");
                return null;
            }

            var registeredDesc = AdaptivePerformanceSubsystemDescriptor.RegisterDescriptor(new AdaptivePerformanceSubsystemDescriptor.Cinfo
            {
                id = "GoogleAndroid",
                providerType = typeof(GoogleAndroidAdaptivePerformanceSubsystem.GoogleAndroidAdaptivePerformanceSubsystemProvider),
                subsystemTypeOverride = typeof(GoogleAndroidAdaptivePerformanceSubsystem)
            });
            return registeredDesc;
        }

        public class GoogleAndroidAdaptivePerformanceSubsystemProvider : APProvider, IApplicationLifecycle, IDevicePerformanceLevelControl
        {
            NativeApi m_Api = null;

            PerformanceDataRecord m_Data = new PerformanceDataRecord();
            object m_DataLock = new object();

            float m_Temperature;
            float m_TemperatureUpdateTimestamp;

            Version m_Version = null;
            PerformanceMode m_PerformanceMode = PerformanceMode.Unknown;

            public override IApplicationLifecycle ApplicationLifecycle { get { return this; } }
            public override IDevicePerformanceLevelControl PerformanceLevelControl { get { return this; } }

            public int MaxCpuPerformanceLevel { get; set; }
            public int MaxGpuPerformanceLevel { get; set; }

            static GoogleAndroidProviderSettings s_Settings = GoogleAndroidProviderSettings.GetSettings();

            public GoogleAndroidAdaptivePerformanceSubsystemProvider()
            {
                MaxCpuPerformanceLevel = 3;
                MaxGpuPerformanceLevel = 3;

                m_Api = new NativeApi(OnPerformanceWarning);
            }

            void OnPerformanceWarning(WarningLevel warningLevel)
            {
                lock (m_DataLock)
                {
                    m_Data.ChangeFlags |= Feature.WarningLevel;
                    m_Data.WarningLevel = warningLevel;
                }
            }

            void ImmediateUpdateTemperature()
            {
                if (!Capabilities.HasFlag(Feature.TemperatureLevel))
                    return;

                UpdateTemperatureLevel();
                m_TemperatureUpdateTimestamp = Time.time;

                lock (m_DataLock)
                {
                    m_Data.ChangeFlags |= Feature.TemperatureLevel;
                    m_Data.TemperatureLevel = m_Temperature;
                }
            }

            void TimedUpdateTemperature()
            {
                if (!Capabilities.HasFlag(Feature.TemperatureLevel))
                    return;

                var timestamp = Time.time;
                var canUpdateTemperature = (timestamp - m_TemperatureUpdateTimestamp) > 1.0f;

                if (!canUpdateTemperature)
                    return;

                var previousTemperature = m_Temperature;
                UpdateTemperatureLevel();
                var isTemperatureChanged = previousTemperature != m_Temperature;
                m_TemperatureUpdateTimestamp = timestamp;

                if (!isTemperatureChanged)
                    return;

                lock (m_DataLock)
                {
                    m_Data.ChangeFlags |= Feature.TemperatureLevel;
                    m_Data.TemperatureLevel = m_Temperature;
                }
            }

            void ImmediateUpdateThermalStatus()
            {
                if (!Capabilities.HasFlag(Feature.WarningLevel))
                    return;

                var warningLevel = m_Api.GetThermalStatusWarningLevel();

                lock (m_DataLock)
                {
                    m_Data.ChangeFlags |= Feature.WarningLevel;
                    m_Data.WarningLevel = warningLevel;
                }
            }

            void ImmediateUpdatePerformanceMode()
            {
                if (!Capabilities.HasFlag(Feature.PerformanceMode))
                    return;

#if UNITY_2023_1_OR_NEWER
                var gameMode = m_Api.GetGameMode();
                m_PerformanceMode = PerformanceModeUtilities.ConvertGameModeToPerformanceMode(gameMode);
#else
                m_PerformanceMode = PerformanceMode.Unknown;
#endif

                if (m_Data.PerformanceMode == m_PerformanceMode)
                    return;

                lock (m_DataLock)
                {
                    m_Data.ChangeFlags |= Feature.PerformanceMode;
                    m_Data.PerformanceMode = m_PerformanceMode;
                }
            }

            static bool TryParseVersion(string versionString, out Version version)
            {
                try
                {
                    version = new Version(versionString);
                }
                catch (Exception)
                {
                    version = null;
                    return false;
                }
                return true;
            }

            protected override bool TryInitialize()
            {
                if (Initialized)
                {
                    return true;
                }

                if (!base.TryInitialize())
                {
                    return false;
                }

                if (!m_Api.Initialize())
                {
                    return false;
                }

                if (TryParseVersion(m_Api.GetVersion(), out m_Version))
                {
                    if (m_Version >= new Version(11, 0))
                    {
                        Initialized = true;
                        MaxCpuPerformanceLevel = m_Api.GetMaxCpuPerformanceLevel();
                        MaxGpuPerformanceLevel = m_Api.GetMaxGpuPerformanceLevel();
                        Capabilities = Feature.CpuPerformanceLevel | Feature.GpuPerformanceLevel | Feature.WarningLevel;
                    }
                    if (m_Version >= new Version(12, 0))
                    {
                        Capabilities |= Feature.PerformanceMode;
                        Capabilities |= Feature.TemperatureTrend;
                        Capabilities |= Feature.TemperatureLevel;
                        Capabilities |= Feature.PerformanceLevelControl;
                    }

                    if (m_Version < new Version(11, 0))
                    {
                        m_Api.Terminate();
                        Initialized = false;
                    }
                }

                m_Data.PerformanceLevelControlAvailable = true;

                return Initialized;
            }

            public override void Start()
            {
                if (!Initialized)
                {
                    return;
                }

                if (m_Running)
                {
                    return;
                }

                ImmediateUpdateTemperature();
                ImmediateUpdateThermalStatus();
                ImmediateUpdatePerformanceMode();

                m_Running = true;

                // check for availability of the APIS since some devices don't report thermals at all
                if (double.IsNaN(m_Api.GetThermalHeadroom()) && m_Api.GetThermalStatusWarningLevel() == WarningLevel.NoWarning)
                {
                    ADPFLog.Debug("This device does not report thermal headroom correctly. Shutting down Adaptive Performance.)");
                    Destroy();
                }
            }

            public override void Stop()
            {
                m_Running = false;
            }

            public override void Destroy()
            {
                if (m_Running)
                {
                    Stop();
                }

                if (Initialized)
                {
                    m_Api.Terminate();
                    Initialized = false;
                }
            }

            public override string Stats => $"SkinTemp={m_Temperature} PerformanceMode={m_PerformanceMode}";

            public override PerformanceDataRecord Update()
            {
                if(Capabilities.HasFlag(Feature.PerformanceLevelControl))
                    m_Api.UpdateHintSystem();

                ImmediateUpdatePerformanceMode();

                TimedUpdateTemperature();

                lock (m_DataLock)
                {
                    PerformanceDataRecord result = m_Data;
                    m_Data.ChangeFlags = Feature.None;

                    return result;
                }
            }

            public override Version Version
            {
                get
                {
                    return m_Version;
                }
            }

            public override Feature Capabilities { get; set; }

            public override bool Initialized { get; set; }

            public bool SetPerformanceLevel(ref int cpuLevel, ref int gpuLevel)
            {
                return false;
            }

            public bool EnableCpuBoost()
            {
                return false;
            }

            public bool EnableGpuBoost()
            {
                return false;
            }

            public void ApplicationPause() { }

            public void ApplicationResume()
            {
                ImmediateUpdateTemperature();
                ImmediateUpdatePerformanceMode();
            }

            void UpdateTemperatureLevel()
            {
                if (!Capabilities.HasFlag(Feature.TemperatureLevel))
                    return;
                m_Temperature = (float)Math.Round((float)m_Api.GetThermalHeadroom(), 2, MidpointRounding.AwayFromZero);
            }

            internal class NativeApi
            {
                const int k_TemperatureWarningLevelErrorOrUnknown = -1;
                const int k_TemperatureWarningLevelNoWarning = 0;
                const int k_TemperatureWarningLevelThrottlingImminent = 1;
                const int k_TemperatureWarningLevelThrottling = 2;

                static bool s_IsAvailable = false;

                static Action<WarningLevel> s_PerformanceWarningEvent;

                public NativeApi(Action<WarningLevel> sustainedPerformanceWarning)
                {
                    s_PerformanceWarningEvent = sustainedPerformanceWarning;
                    StaticInit();
                }

                /// <summary>
                /// A delegate representation of <see cref="OnHighTempWarning(int)"/>. This maintains a strong
                /// reference to the delegate, which is converted to an IntPtr by <see cref="m_OnHighTempWarningHandlerFuncPtr"/>.
                /// </summary>
                static Action<int> s_OnHighTempWarningHandler = OnHighTempWarning;

                /// <summary>
                /// A pointer to a method to be called immediately when thermal state changes.
                /// </summary>
                readonly IntPtr m_OnHighTempWarningHandlerFuncPtr = Marshal.GetFunctionPointerForDelegate(s_OnHighTempWarningHandler);

                [Preserve]
                [MonoPInvokeCallback(typeof(Action<int>))]
                static void OnHighTempWarning(int warningLevel)
                {
                    ADPFLog.Debug("Listener: onHighTempWarning(warningLevel={0})", warningLevel);

                    if (warningLevel == k_TemperatureWarningLevelNoWarning || warningLevel == k_TemperatureWarningLevelErrorOrUnknown)
                        s_PerformanceWarningEvent(WarningLevel.NoWarning);
                    else if (warningLevel == k_TemperatureWarningLevelThrottlingImminent)
                        s_PerformanceWarningEvent(WarningLevel.ThrottlingImminent);
                    else if (warningLevel == k_TemperatureWarningLevelThrottling)
                        s_PerformanceWarningEvent(WarningLevel.Throttling);
                }

                static void StaticInit()
                {
                    if (s_IsAvailable)
                        return;

                    s_IsAvailable = true;
                }

                public static bool IsAvailable()
                {
                    StaticInit();
                    return s_IsAvailable;
                }

                [DllImport("AdaptivePerformanceThermalHeadroom", EntryPoint = "Unity_AdaptivePerformance_ThermalHeadroom_Setup")]
                public static extern void ThermalHeadroomSetup();
                [DllImport("AdaptivePerformanceThermalHeadroom", EntryPoint = "Unity_AdaptivePerformance_ThermalHeadroom_Teardown")]
                public static extern void ThermalHeadroomTeardown();
                [DllImport("AdaptivePerformanceThermalHeadroom", EntryPoint = "Unity_AdaptivePerformance_ThermalHeadroom_GetThermalHeadroomForSeconds")]
                public static extern double GetThermalHeadroomForSeconds(int forecastSeconds);

                [DllImport("AdaptivePerformanceAndroid", EntryPoint = "Unity_AdaptivePerformance_ThermalHeadroom_Setup")]
                public static extern void ThermalSetup(IntPtr _onHighTempWarning);
                [DllImport("AdaptivePerformanceAndroid", EntryPoint = "Unity_AdaptivePerformance_ThermalHeadroom_Teardown")]
                public static extern void ThermalTeardown();
                [DllImport("AdaptivePerformanceAndroid", EntryPoint = "Unity_AdaptivePerformance_ThermalHeadroom_GetLatestThermalStatus")]
                public static extern int GetLatestThermalStatus();
                [DllImport("AdaptivePerformanceAndroid", EntryPoint = "Unity_AdaptivePerformance_ThermalHeadroom_GetPluginCallback")]
                public static extern IntPtr GetThermalPluginCallback();

                [DllImport("AdaptivePerformanceHint", EntryPoint = "Unity_AdaptivePerformance_Hint_Setup")]
                public static extern void HintSetup(int desiredDuration);
                [DllImport("AdaptivePerformanceHint", EntryPoint = "Unity_AdaptivePerformance_Hint_Teardown")]
                public static extern void HintTeardown();
                [DllImport("AdaptivePerformanceHint", EntryPoint = "Unity_AdaptivePerformance_Hint_GetPreferredUpdateRate")]
                public static extern int GetPreferredUpdateRate();
                [DllImport("AdaptivePerformanceHint", EntryPoint = "Unity_AdaptivePerformance_Hint_ReportCompletionTime")]
                public static extern void ReportCompletionTime(int completionTime);
                [DllImport("AdaptivePerformanceHint", EntryPoint = "Unity_AdaptivePerformance_Hint_UpdateTargetWorkDuration")]
                public static extern void UpdateTargetWorkDuration(int targetDuration);
                [DllImport("AdaptivePerformanceHint", EntryPoint = "Unity_AdaptivePerformance_Hint_GetPluginCallback")]
                public static extern IntPtr GetHintPluginCallback();

                public bool SetupThermalHeadroom()
                {
                    bool success = false;
                    try
                    {
                        ThermalHeadroomSetup();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        ADPFLog.Debug($"[Exception] RegisterListener() thermal headroom failed with {ex}!");
                    }


                    if (!success)
                        ADPFLog.Debug($"failed to register thermal headroom");

                    return success;
                }

                public bool SetupThermal ()
                {
                    bool success = false;
                    try
                    {
                        ThermalSetup(m_OnHighTempWarningHandlerFuncPtr);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        ADPFLog.Debug($"[Exception] RegisterListener() Thermal failed with {ex}!");
                    }


                    if (!success)
                        ADPFLog.Debug($"failed to register thermal");

                    return success;
                }

                public bool SetupHints()
                {
                    bool success = false;
                    CommandBuffer commandBuffer = new CommandBuffer();
                    commandBuffer.IssuePluginEventAndData(GetHintPluginCallback(), 0, (IntPtr)0);
                    Graphics.ExecuteCommandBuffer(commandBuffer);

                    try
                    {
                        var desiredDuration = 16666666; // nanoseconds
                        if (Application.targetFrameRate >= 0)
                            desiredDuration = (int)(1.0 / (double)Application.targetFrameRate * 1000000000.0);
                        HintSetup(desiredDuration);
                        ADPFLog.Debug($" Preferred update rate:  {GetPreferredUpdateRate()} desired duration: {desiredDuration}!");
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        ADPFLog.Debug($"[Exception] RegisterListener() Hint Setup failed with {ex}!");
                    }

                    if (!success)
                        ADPFLog.Debug($"failed to register hints");

                    return success;
                }

                public bool Initialize()
                {
                    bool isInitialized = false;
                    try
                    {
                        Version initVersion;
                        if (TryParseVersion(GetVersion(), out initVersion))
                        {
                            if (initVersion >= new Version(11, 0))
                            {
                                isInitialized = true;
                                SetupThermal();
                            }
                            if (initVersion >= new Version(12, 0))
                            {
                                SetupThermalHeadroom();
                                SetupHints();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ADPFLog.Debug($"[Exception] Initialize() failed due to {ex}!");
                    }

                    return isInitialized;
                }

                public void Terminate()
                {
                    try
                    {
                        Version initVersion;
                        if (TryParseVersion(GetVersion(), out initVersion))
                        {
                            if (initVersion >= new Version(11, 0))
                            {
                                ThermalTeardown();
                            }
                            if (initVersion >= new Version(12, 0))
                            {
                                HintTeardown();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ADPFLog.Debug($"[Exception] Terminate() failed due to {ex}!");
                    }
                }

                public string GetVersion()
                {
                    string sdkVersion = "0.0.0";
                    try
                    {
                        string systemInfo = SystemInfo.operatingSystem;
                        sdkVersion = systemInfo.Substring(systemInfo.IndexOf("OS ") + 3, 2)+".0.0";
                        ADPFLog.Debug($"OS: {sdkVersion}");
                    }
                    catch (Exception ex)
                    {
                        ADPFLog.Debug($"[Exception] getVersion() failed! {ex}");
                    }
                    return sdkVersion;
                }

                public double GetThermalHeadroom(int forecastInSeconds = 0)
                {
                    return GetThermalHeadroomForSeconds(forecastInSeconds);
                }

                public void UpdateHintSystem()
                {
                    UnityEngine.FrameTiming[] frameTimings = new UnityEngine.FrameTiming[1];
                    FrameTimingManager.CaptureFrameTimings();
                    uint res = FrameTimingManager.GetLatestTimings(1, frameTimings);
                    if (res > 0 && frameTimings != null)
                    {
#if UNITY_2022_1_OR_NEWER
                        var frameTimeInInt = (int)((frameTimings[0].cpuMainThreadFrameTime + frameTimings[0].cpuRenderThreadFrameTime) * 1000000.0);
#else
                        var frameTimeInInt = (int)((frameTimings[0].cpuFrameTime) * 1000000.0);
#endif
                        var desiredDuration = 16666666;
                        if (OnDemandRendering.effectiveRenderFrameRate > 0)
                            desiredDuration = (int)((1.0 / (double)OnDemandRendering.effectiveRenderFrameRate) * 1000000000.0);

                        ReportCompletionTime(frameTimeInInt);
                        UpdateTargetWorkDuration(desiredDuration);
                    }
                    else
                    {
                        ADPFLog.Debug($"FrameTimingManager does not have results, skip reporting.");
                    }
                }

                public WarningLevel GetThermalStatusWarningLevel()
                {
                    var thermalStatus = GetLatestThermalStatus();
                    var warningLevel = WarningLevel.NoWarning;

                    if (thermalStatus == k_TemperatureWarningLevelThrottlingImminent)
                        warningLevel = WarningLevel.ThrottlingImminent;
                    else if (thermalStatus == k_TemperatureWarningLevelThrottling)
                        warningLevel = WarningLevel.Throttling;

                    return warningLevel;
                }

#if UNITY_2023_1_OR_NEWER
                public AndroidGameMode GetGameMode()
                {
                    return AndroidGame.GameMode;
                }
#endif

                public bool EnableCpuBoost()
                {
                    return false;
                }

                public bool EnableGpuBoost()
                {
                    return false;
                }

                public int GetClusterInfo()
                {
                    int result = -999;
                    return result;
                }

                public int GetMaxCpuPerformanceLevel()
                {
                    int maxCpuPerformanceLevel = -1;
                    return maxCpuPerformanceLevel;
                }

                public int GetMaxGpuPerformanceLevel()
                {
                    int maxGpuPerformanceLevel = -1;
                    return maxGpuPerformanceLevel;
                }
            }
        }
    }
}
#endif
