using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AdaptivePerformance;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AdaptivePerformance.Editor;
#endif
using System.Runtime.InteropServices;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance.Google.Android
{
    /// <summary>
    /// GoogleAndroidProviderLoader implements the loader for Adaptive Performance on devices running Android.
    /// </summary>
#if UNITY_EDITOR
    [AdaptivePerformanceSupportedBuildTargetAttribute(BuildTargetGroup.Android)]
#endif
    public class GoogleAndroidProviderLoader : AdaptivePerformanceLoaderHelper
    {
        static List<AdaptivePerformanceSubsystemDescriptor> s_GoogleAndroidSubsystemDescriptors =
            new List<AdaptivePerformanceSubsystemDescriptor>();

        /// <summary>
        /// Returns if the provider loader was initialized successfully.
        /// </summary>
        public override bool Initialized
        {
            get
            {
#if UNITY_ANDROID
                return googleAndroidSubsystem != null;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Returns if the provider loader is currently running.
        /// </summary>
        public override bool Running
        {
            get
            {
#if UNITY_ANDROID
                return googleAndroidSubsystem != null && googleAndroidSubsystem.running;
#else
                return false;
#endif
            }
        }

#if UNITY_ANDROID
        /// <summary>Returns the currently active Android Subsystem instance, if an instance exists.</summary>
        public GoogleAndroidAdaptivePerformanceSubsystem googleAndroidSubsystem
        {
            get { return GetLoadedSubsystem<GoogleAndroidAdaptivePerformanceSubsystem>(); }
        }
#endif
        /// <summary>
        /// Implementation of <see cref="AdaptivePerformanceLoader.GetDefaultSubsystem"/>.
        /// </summary>
        /// <returns>Returns the Android Subsystem, which is the loaded default subsystem. Because only one subsystem can be present at a time, Adaptive Performance always initializes the first subsystem and uses it as a default. You can change subsystem order in the Adaptive Performance Provider Settings.</returns>
        public override ISubsystem GetDefaultSubsystem()
        {
#if UNITY_ANDROID
            return googleAndroidSubsystem;
#else
            return null;
#endif
        }

        /// <summary>
        /// Implementation of <see cref="AdaptivePerformanceLoader.GetSettings"/>.
        /// </summary>
        /// <returns>Returns the Android settings.</returns>
        public override IAdaptivePerformanceSettings GetSettings()
        {
            return GoogleAndroidProviderSettings.GetSettings();
        }

        /// <summary>Implementation of <see cref="AdaptivePerformanceLoader.Initialize"/>.</summary>
        /// <returns>True if successfully initialized the Android subsystem, false otherwise.</returns>
        public override bool Initialize()
        {
#if UNITY_ANDROID
            CreateSubsystem<AdaptivePerformanceSubsystemDescriptor, GoogleAndroidAdaptivePerformanceSubsystem>(s_GoogleAndroidSubsystemDescriptors, "GoogleAndroid");
            if (googleAndroidSubsystem == null)
            {
                Debug.LogError("Unable to start the Android subsystem.");
            }

            return googleAndroidSubsystem != null;
#else
            return false;
#endif
        }

        /// <summary>Implementation of <see cref="AdaptivePerformanceLoader.Start"/>.</summary>
        /// <returns>True if successfully started the Android subsystem, false otherwise.</returns>
        public override bool Start()
        {
#if UNITY_ANDROID
            StartSubsystem<GoogleAndroidAdaptivePerformanceSubsystem>();
            return true;
#else
            return false;
#endif
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Stop"/>.</summary>
        /// <returns>True if successfully stopped the Android subsystem, false otherwise.</returns>
        public override bool Stop()
        {
#if UNITY_ANDROID
            StopSubsystem<GoogleAndroidAdaptivePerformanceSubsystem>();
            return true;
#else
            return false;
#endif
        }

        /// <summary>Implementaion of <see cref="AdaptivePerformanceLoader.Deinitialize"/>.</summary>
        /// <returns>True if successfully deinitialized the Android subsystem, false otherwise.</returns>
        public override bool Deinitialize()
        {
#if UNITY_ANDROID
            DestroySubsystem<GoogleAndroidAdaptivePerformanceSubsystem>();
            return base.Deinitialize();
#else
            return false;
#endif
        }
    }
}
