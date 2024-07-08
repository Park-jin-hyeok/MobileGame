using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace UnityEngine.AdaptivePerformance.Google.Android
{
    /// <summary>
    /// Provider Settings for Android provider, which controls the runtime asset instance that stores the settings.
    /// </summary>
    [System.Serializable]
    [AdaptivePerformanceConfigurationData("Android", GoogleAndroidProviderConstants.k_SettingsKey)]
    public class GoogleAndroidProviderSettings : IAdaptivePerformanceSettings
    {
        [SerializeField, Tooltip("Enable Logging in Devmode")]
        bool m_GoogleProviderLogging = false;

        /// <summary>
        ///  Control debug logging of the Android provider.
        ///  This setting only affects development builds. All logging is disabled in release builds.
        ///  You can also control the global logging setting after startup by using <see cref="IDevelopmentSettings.Logging"/>.
        ///  Logging is disabled by default.
        /// </summary>
        /// <value>Set this to true to enable debug logging, or false to disable it (default: false).</value>
        public bool googleProviderLogging
        {
            get { return m_GoogleProviderLogging; }
            set { m_GoogleProviderLogging = value; }
        }

        /// <summary>Static instance that holds the runtime asset instance Unity creates during the build process.</summary>
#if !UNITY_EDITOR
        public static GoogleAndroidProviderSettings s_RuntimeInstance = null;
#endif
        void Awake()
        {
#if !UNITY_EDITOR
            s_RuntimeInstance = this;
#endif
        }

        /// <summary>
        /// Returns Android Provider Settings which are used by Adaptive Performance to apply Provider Settings.
        /// </summary>
        /// <returns>Android Provider Settings</returns>
        public static GoogleAndroidProviderSettings GetSettings()
        {
            GoogleAndroidProviderSettings settings = null;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject<GoogleAndroidProviderSettings>(GoogleAndroidProviderConstants.k_SettingsKey, out settings);
#else
            settings = s_RuntimeInstance;
#endif
            return settings;
        }
    }
}
