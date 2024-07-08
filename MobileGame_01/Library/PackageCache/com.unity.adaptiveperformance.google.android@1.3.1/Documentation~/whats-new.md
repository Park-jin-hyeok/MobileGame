# What's new in version 1.0.0
Summary of changes in Adaptive Performance Android provider version 1.0.0.

The main updates in this release include:

## Removed

## Added
* Support for Android SDK 33
* Integrated Android APIs
  * [GameManager](https://developer.android.com/reference/android/app/GameManager) APIs
    * [getGameMode](https://developer.android.com/reference/android/app/GameManager#getGameMode())
    * [setGameState](https://developer.android.com/reference/android/app/GameManager#setGameState(android.app.GameState))
  * [PowerManager](https://developer.android.com/reference/android/os/PowerManager) APIs
    * [getThermalHeadroom](https://developer.android.com/reference/android/os/PowerManager#getThermalHeadroom(int))
    * [getCurrentThermalStatus](https://developer.android.com/reference/android/os/PowerManager#getCurrentThermalStatus())
    * [ThermalStatusChangedListener](https://developer.android.com/reference/android/os/PowerManager.OnThermalStatusChangedListener)
  * [PerformanceHintManager](https://developer.android.com/reference/android/os/PerformanceHintManager) APIs
    * [createHintSession](https://developer.android.com/reference/android/os/PerformanceHintManager#createHintSession(int[],%20long))
    * [getPreferredUpdateRateNanos](https://developer.android.com/reference/android/os/PerformanceHintManager#getPreferredUpdateRateNanos())
  * [PerformanceHintManager.Session](https://developer.android.com/reference/android/os/PerformanceHintManager.Session) APIs
    * [reportActualWorkDuration](https://developer.android.com/reference/android/os/PerformanceHintManager.Session#reportActualWorkDuration(long))
* Documentation

## Updated

## Fixed

For a full list of changes and updates in this version, see the [Adaptive Performance Android provider changelog](../changelog/CHANGELOG.html).
