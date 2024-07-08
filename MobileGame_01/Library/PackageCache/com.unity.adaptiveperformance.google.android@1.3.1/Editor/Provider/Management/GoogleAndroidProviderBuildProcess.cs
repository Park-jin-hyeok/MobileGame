using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine.AdaptivePerformance.Google.Android;

namespace UnityEditor.AdaptivePerformance.Google.Android.Editor
{
    /// <summary>
    /// The Android provider build processor ensures that any custom configuration that the user creates is
    /// correctly passed on to the provider implementation at runtime.
    ///
    /// Custom configuration instances that are stored in EditorBuildSettings are not copied to the target build
    /// because they are considered unreferenced assets. To get them to the runtime, they must
    /// be serialized to the built app and deserialized at runtime. Previously, this as a manual process
    /// that required the implementor to manually serialize to some location that can then be read from to deserialize
    /// at runtime. With the new PlayerSettings Preloaded Assets API, you can now add your asset to the preloaded
    /// list and have it be instantiated at app launch.
    ///
    /// **Note**: Preloaded assets are only notified with Awake, so anything you want or need to do with the
    /// asset after launch needs to be handled in the Android provider build process.
    ///
    /// For more information about the APIs used, see:
    /// * <see href="https://docs.unity3d.com/ScriptReference/EditorBuildSettings.html">EditorBuildSettings</see>
    /// * <see href="https://docs.unity3d.com/ScriptReference/PlayerSettings.GetPreloadedAssets.html">PlayerSettings.GetPreloadedAssets</see>
    /// * <see href="https://docs.unity3d.com/ScriptReference/PlayerSettings.SetPreloadedAssets.html">PlayerSettings.SetPreloadedAssets</see>
    /// * <see href="https://docs.unity3d.com/ScriptReference/Android.IPostGenerateGradleAndroidProject.html">IPostGenerateGradleAndroidProject</see>
    /// </summary>
    public class GoogleAndroidProviderBuildProcess : IPreprocessBuildWithReport, IPostprocessBuildWithReport, IPostGenerateGradleAndroidProject
    {
        /// <summary>
        /// Override of <see cref="IPreprocessBuildWithReport"/> and <see cref="IPostprocessBuildWithReport"/>.
        /// </summary>
        public int callbackOrder
        {
            get { return 0; }
        }

        /// <summary>
        /// Clears out settings which could be left over from previous unsuccessfull runs.
        /// </summary>
        void CleanOldSettings()
        {
            UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (preloadedAssets == null)
                return;

            var oldSettings = from s in preloadedAssets
                              where s != null && s.GetType() == typeof(GoogleAndroidProviderSettings)
                              select s;

            if (oldSettings != null && oldSettings.Any())
            {
                var assets = preloadedAssets.ToList();
                foreach (var s in oldSettings)
                {
                    assets.Remove(s);
                }

                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        List<PluginImporter> m_DisabledPlugins = new List<PluginImporter>();

        void ExcludeNativeLibsWhenProviderDisabled()
        {
            var generalSettings = AdaptivePerformanceGeneralSettingsPerBuildTarget.AdaptivePerformanceGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            foreach (var loader in generalSettings.AssignedSettings.loaders)
            {
                if (loader is GoogleAndroidProviderLoader)
                {
                    return;
                }
            }

            foreach (var p in PluginImporter.GetImporters(BuildTarget.Android))
            {
                if (p.ShouldIncludeInBuild() && p.assetPath.Contains("AdaptivePerformanceAndroid"))
                {
                    p.SetIncludeInBuildDelegate(path => false);
                    m_DisabledPlugins.Add(p);
                }
            }
        }

        void RestoreNativeLibs()
        {
            foreach (var p in m_DisabledPlugins)
            {
                p.SetIncludeInBuildDelegate(path => true);
            }
            m_DisabledPlugins.Clear();
        }

        /// <summary>
        /// Override of <see cref="IPreprocessBuildWithReport"/>.
        /// </summary>
        /// <param name="report">Build report.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            ExcludeNativeLibsWhenProviderDisabled();

            // Always remember to clean up preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();

            GoogleAndroidProviderSettings settings = null;
            EditorBuildSettings.TryGetConfigObject(GoogleAndroidProviderConstants.k_SettingsKey, out settings);
            if (settings == null)
                return;

            UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

            if (!preloadedAssets.Contains(settings))
            {
                var assets = preloadedAssets.ToList();
                assets.Add(settings);
                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        /// <summary>Override of <see cref="IPostprocessBuildWithReport"/></summary>.
        /// <param name="report">Build report.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            // Always remember to clean up preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();

            RestoreNativeLibs();
        }

        /// <summary>
        /// Implementation of <see cref="IPostGenerateGradleAndroidProject"/>
        /// </summary>
        /// <param name="path"></param>
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var setting = AdaptivePerformanceBuildUtils.GetWantedStartupBoostSetting(GoogleAndroidProviderSettings.GetSettings());
            AdaptivePerformanceBuildUtils.UpdateBootConfigBoostSetting(path, "adaptive-performance-google-boost-launch", setting);
        }
    }
}
