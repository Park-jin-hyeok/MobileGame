# Requirements

This page lists the minimum requirements you need to use the Adaptive Performance Android provider.

## Unity Editor

This version of the Adaptive Performance Android provider requires at least Unity Editor version 2023.1.

## Device support

Adaptive Performance Android currently supports devices that run Android 12+.

The following features currently only run on Google Pixel 6+ models and require Android 13.

* [GameManager](https://developer.android.com/reference/android/app/GameManager) APIs
  * [getGameMode](https://developer.android.com/reference/android/app/GameManager#getGameMode())
  * [setGameState](https://developer.android.com/reference/android/app/GameManager#setGameState(android.app.GameState))

## Android and Unity Version support

| **Android API**              | **Android** | Unity (LTS)<br/>**2020.3** | Unity (LTS)<br/>**2021.3** | Unity (LTS)<br/>**2022.2** | Unity (Preview)<br/>**2023.1** |
|:-----------------------------|:-----------:|:--------------------------:|:--------------------------:|:--------------------------:|:------------------------------:|
| ADPF - Thermal APIs          |   11 (R)    |             N              |           **Y**            |           **Y**            |             **Y**              |
| ADPF - Performance Hint APIs |   12 (S)    |             N              |           **Y**            |           **Y**            |             **Y**              |
| GameMode APIs                |   13 (T)    |             N              |             N              |             N              |             **Y**              |
| GameState APIs               |   13 (T)    |             N              |             N              |             N              |             **Y**              |
