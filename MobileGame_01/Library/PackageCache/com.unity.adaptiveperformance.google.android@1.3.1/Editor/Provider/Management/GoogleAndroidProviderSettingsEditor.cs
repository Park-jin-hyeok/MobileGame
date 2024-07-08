using UnityEngine;
using UnityEditor.AdaptivePerformance.Editor;

using UnityEngine.AdaptivePerformance.Google.Android;

namespace UnityEditor.AdaptivePerformance.Google.Android.Editor
{
    /// <summary>
    /// This is custom Editor for Android Provider Settings.
    /// </summary>
    [CustomEditor(typeof(GoogleAndroidProviderSettings))]
    public class GoogleAndroidProviderSettingsEditor : ProviderSettingsEditor
    {
        const string k_GoogleProviderLogging = "m_GoogleProviderLogging";

        static GUIContent s_GoogleProviderLoggingLabel = EditorGUIUtility.TrTextContent(L10n.Tr("Android Provider Logging"), L10n.Tr("Only active in development mode."));

        static string s_UnsupportedInfo = L10n.Tr("Adaptive Performance Android settings not available on this platform.");
        SerializedProperty m_GoogleProviderLoggingProperty;

        /// <summary>
        /// Override of provider options to indicate if boost is available.
        /// </summary>
        protected override bool IsBoostAvailable => false;
        /// <summary>
        /// Override of provider options to indicate if Auto Performance mode is available.
        /// </summary>
        protected override bool IsAutoPerformanceModeAvailable => false;

#if UNITY_2023_1_OR_NEWER
        /// <summary>
        /// Controls whether or not the 'AutomaticGameModeEnabled' option is available. Default value is <c>false</c>.
        /// </summary>
        protected override bool IsAutoGameModeAvailable => true;
#endif

        /// <summary>
        /// Override of Editor callback to display custom settings.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (!DisplayBaseSettingsBegin())
                return;

            if (m_GoogleProviderLoggingProperty == null)
                m_GoogleProviderLoggingProperty = serializedObject.FindProperty(k_GoogleProviderLogging);

            BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();

            if (selectedBuildTargetGroup == BuildTargetGroup.Android)
            {
                EditorGUIUtility.labelWidth = 180; // some property labels are cut-off
                DisplayBaseRuntimeSettings();
                EditorGUILayout.Space();

                DisplayBaseDeveloperSettings();
                if (m_ShowDevelopmentSettings)
                {
                    EditorGUI.indentLevel++;
                    GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;
                    EditorGUILayout.PropertyField(m_GoogleProviderLoggingProperty, s_GoogleProviderLoggingLabel);
                    GUI.enabled = true;
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.HelpBox(s_UnsupportedInfo, MessageType.Info);
                EditorGUILayout.Space();
            }
            DisplayBaseSettingsEnd();
        }
    }
}
