using System.Collections.Generic;
using UnityEngine.AdaptivePerformance.Google.Android;
using UnityEditor.AdaptivePerformance.Editor.Metadata;
using UnityEngine;

namespace UnityEditor.AdaptivePerformance.Google.Android.Editor
{
    internal class GoogleAndroidProviderMetadata : IAdaptivePerformancePackage
    {
        private class GoogleAndroidPackageMetadata : IAdaptivePerformancePackageMetadata
        {
            public string packageName => "Adaptive Performance Android";
            public string packageId => "com.unity.adaptiveperformance.google.android";
            public string settingsType => "UnityEngine.AdaptivePerformance.Google.Android.GoogleAndroidProviderSettings";
            public string licenseURL => "https://docs.unity3d.com/Packages/com.unity.adaptiveperformance.google.android@latest?subfolder=/license/LICENSE.html";
            public List<IAdaptivePerformanceLoaderMetadata> loaderMetadata => s_LoaderMetadata;

            private readonly static List<IAdaptivePerformanceLoaderMetadata> s_LoaderMetadata = new List<IAdaptivePerformanceLoaderMetadata>() { new GoogleLoaderMetadata() };
        }

        private class GoogleLoaderMetadata : IAdaptivePerformanceLoaderMetadata
        {
            public string loaderName => "Android Provider";
            public string loaderType => "UnityEngine.AdaptivePerformance.Google.Android.GoogleAndroidProviderLoader";
            public List<BuildTargetGroup> supportedBuildTargets => s_SupportedBuildTargets;

            private readonly static List<BuildTargetGroup> s_SupportedBuildTargets = new List<BuildTargetGroup>()
            {
                BuildTargetGroup.Android
            };
        }

        private static IAdaptivePerformancePackageMetadata s_Metadata = new GoogleAndroidPackageMetadata();
        public IAdaptivePerformancePackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            var settings = obj as GoogleAndroidProviderSettings;
            if (settings != null)
            {
                settings.logging = false;
                settings.statsLoggingFrequencyInFrames = 50;
                settings.automaticPerformanceMode = true;

                return true;
            }

            return false;
        }
    }
}
