# Adaptive Performance Android provider

The Adaptive Performance Android provider extends [Adaptive Performance](https://docs.unity3d.com/Packages/com.unity.adaptiveperformance@latest/index.html) to Android devices. To do this, it sends device-specific information from Android devices to the Adaptive Performance package. This enables you to retrieve data about the thermal state and performance mode of Android devices.

This package integrates with the following Android frameworks and APIs:
* Android Dynamic Performance Framework (ADPF) ([more info](https://developer.android.com/games/optimize/adpf))
  * The following APIs are integrated into Adaptive Performance `IThermalStatus` to retrieve thermal information about the device.
    * [getThermalHeadroom](https://developer.android.com/reference/android/os/PowerManager#getThermalHeadroom(int))
    * [getCurrentThermalStatus](https://developer.android.com/reference/android/os/PowerManager#getCurrentThermalStatus())
    * [ThermalStatusChangedListener](https://developer.android.com/reference/android/os/PowerManager.OnThermalStatusChangedListener)
  * The following APIs are integrated into the game loop to retrieve and send performance related details to Android.
    * [createHintSession](https://developer.android.com/reference/android/os/PerformanceHintManager#createHintSession(int[],%20long))
    * [getPreferredUpdateRateNanos](https://developer.android.com/reference/android/os/PerformanceHintManager#getPreferredUpdateRateNanos())
    * [reportActualWorkDuration](https://developer.android.com/reference/android/os/PerformanceHintManager.Session#reportActualWorkDuration(long))
* GameMode API ([more info](https://developer.android.com/games/optimize/adpf/gamemode/gamemode-api))
  * The Android [GetGameMode](https://developer.android.com/reference/android/app/GameManager#getGameMode()) API is integrated into Adaptive Performance `IPerformanceModeStatus`
  * [IAdaptivePerformanceSettings.automaticGameMode](https://docs.unity3d.com/Packages/com.unity.adaptiveperformance@latest/api/UnityEngine.AdaptivePerformance.IAdaptivePerformanceSettings.html#UnityEngine_AdaptivePerformance_IAdaptivePerformanceSettings_automaticGameMode) property controls whether to adjust the target frame rate according to the current device GameMode setting. You can change this property using the Adaptive Performance Android provider setting **Auto Game Mode** . When the **Auto Game Mode** is turned on, the following adjustments occur:
    * Battery saver mode lowers the target frame rate to 30 fps.
    * Performance mode raises the target frame rate to the highest supported by the display.
    * Standard mode sets the [application target frame rate](https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html) to -1, which instructs player to use the default frame rate. The default frame rate on mobile platforms is 30 fps.
  * Unity API also includes `Android.AndroidGame.GameMode` property.
* GameState API ([more info](https://developer.android.com/games/optimize/adpf/gamemode/gamestate-api))
  * Unity API includes `Android.AndroidGame.SetGameState` method to make manual calls to the Android [GameManager](https://developer.android.com/reference/android/app/GameManager).

For information on what's new in the latest version of Adaptive Performance Android, see the [Changelog](../changelog/CHANGELOG.html).
