# Getting started

The Adaptive Performance Samsung (Android) provider is only a data provider for the [Adaptive Performance](https://docs.unity3d.com/Packages/com.unity.adaptiveperformance@latest/index.html) package and the only APIs that it provides are for the Samsung device-specific [Variable Refresh Rate](vrr.md) feature. To use adaptive performance with a Samsung Android device, install this package and use the runtime APIs in the main Adaptive Performance package. For more information, see the [Adaptive Performance](https://docs.unity3d.com/Packages/com.unity.adaptiveperformance@latest/index.html) documentation.

## Installation

1. Install Android support to your Unity project. For more information, see [Android environment setup](https://docs.unity3d.com/Manual/android-sdksetup.html).
2. Install the Adaptive Performance package. To do this, follow the instructions in the [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html).
3. Select **Edit** > **Project Settings** > **Adaptive Performance**.
4. Open the **Android Settings** tab and, in the **Providers** section, select **Samsung Android Provider**.

## Log messages

To enable Adaptive Performance log messages in development builds on Samsung devices:

1. Select **Edit** > **Project Settings** > **Adaptive Performance** > **Samsung (Android)**.
2. In the **Development Settings** foldout, enable **Samsung Provider Logging**.
