# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.3.1] - 2024-03-27

### Changed
* Native libraries are automatically excluded from the build if provider is disabled in Adapative Performance settings.

## [1.3.0] - 2024-03-12

### Added
* Enabled a new property on `IAdaptivePerformanceSettings` to automate frame rate control based on the changes in device performance mode.

### Removed
* `SetGameState` call on Adaptive Performance initialization to allow automated `SetGameState` calls from the Unity Player.

## [1.2.1] - 2023-08-28

### Fixed
* Some deviced do not report correct thermal headroom numbers. Disabling Adaptive Performance on those devices.

## [1.2.0] - 2023-08-03

### Changed
* Unity Support Version dropped to 2021

## [1.1.2] - 2023-06-06

### Fixed
* Fixed build error "Frametiming does not contain a definition for 'cpumainthreadframetime' on Unity versions < 2022.1.

## [1.1.1] - 2023-06-14

### Fixed
* Fixed wrong threshold for throttling where Android Moderate throttling reports now as Adaptive Performance throttling instead of throttling imminent.
* Fix time reporting to Android performance hint manager to report main thread and render thread timing correctly.

## [1.1.0] - 2023-04-17

### Changed
* Rework how Android APIs of Android 11, 12, 13 are integrated and used so we can target Android 11 and not only Android 12+ to provide minimal features sets to the platform (thermal events).


### Fixed
* Fixed Android APIs to ensure we run on any Android device not only Google Pixel devices.


## [1.0.0] - 2023-04-10

### Added
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

### Removed
* Removed Project Settings options that are not available in this Provider.

### Changed
* Adjusted labeling of new Android provider to be listed as Android provider.

### Fixed
- Adjusted the loader and subsystem initialization process to allow for falling back to another subsystem if init is not successful.
- Fixed exception in simulator attempting to load game mode on Editor versions prior to 2023.1
