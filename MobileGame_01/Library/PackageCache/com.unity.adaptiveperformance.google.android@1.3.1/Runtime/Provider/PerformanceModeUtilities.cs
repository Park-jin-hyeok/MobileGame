#if UNITY_2023_1_OR_NEWER

using System;
using UnityEngine.Android;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]
namespace UnityEngine.AdaptivePerformance.Google.Android
{
    public static class PerformanceModeUtilities
    {
        public static PerformanceMode ConvertGameModeToPerformanceMode(AndroidGameMode gameMode)
        {
            var performanceMode = PerformanceMode.Unknown;

            switch (gameMode)
            {
                case AndroidGameMode.Standard:
                    performanceMode = PerformanceMode.Standard;
                    break;
                case AndroidGameMode.Performance:
                    performanceMode = PerformanceMode.Optimize;
                    break;
                case AndroidGameMode.Battery:
                    performanceMode = PerformanceMode.Battery;
                    break;
                case AndroidGameMode.Unsupported:
                default:
                    break;
            }

            return performanceMode;
        }

        public static AndroidGameMode ConvertPerformanceModeToGameMode(PerformanceMode performanceMode)
        {
            var gameMode = AndroidGameMode.Unsupported;
            switch (performanceMode)
            {
                case PerformanceMode.CPU:
                case PerformanceMode.GPU:
                case PerformanceMode.Optimize:
                    gameMode = AndroidGameMode.Performance;
                    break;
                case PerformanceMode.Battery:
                    gameMode = AndroidGameMode.Battery;
                    break;
                case PerformanceMode.Standard:
                    gameMode = AndroidGameMode.Standard;
                    break;
                case PerformanceMode.Unknown:
                default:
                    break;
            }

            return gameMode;
        }
    }
}

#endif
