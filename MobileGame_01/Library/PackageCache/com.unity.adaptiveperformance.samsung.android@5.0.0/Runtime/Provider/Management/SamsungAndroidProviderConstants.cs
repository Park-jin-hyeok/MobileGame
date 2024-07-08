using System;

namespace UnityEngine.AdaptivePerformance.Samsung.Android
{
    /// <summary>
    /// Static constants for the Samsung Settings Provider.
    /// </summary>
    public static class SamsungAndroidProviderConstants
    {
        /// <summary>
        /// Key used to store and retrieve custom configuration settings from EditorBuildSettings.
        /// </summary>
        public const string k_SettingsKey = "com.unity.adaptiveperformance.samsung.android.provider_settings";

        /// <summary>
        /// InvalidOperation is the return value of an SDK API call when the feature is not available.
        /// </summary>
        /// <value>-999</value>
        public const int k_InvalidOperation = -999;
    }
}
