Copyright (c) 2018, Sturfee Inc.

Sturfee VPS SDK
version 1.4.4b1

Scene : ArCore

This scene represents how XRSession can be initialized for Android devices that support ArCore. Notice the ProviderSet in SturfeeXRSession inspector is set to `Android/ArCore Provider Set`.


SturfeeXRSession : Initializes XRSession using the the selected provider set in ProviderSet dropdown. 

LocalizationManager : Handles all aspects of localization, including the UI, calls to the Sturfee VPS server, and event handling.

SturfeeUIManager : Provides helpful UI functionality outside of localization.

SturfeeXRCamera : Applies Pose obtained from XRSession to XRCamera. XRCamera GameObject should have the tag `XRCamera`.

SturfeeXRLight : A GameObject with Light component which gets its intensity and color values based on current sun direction for the location provided in GpsProvider.

LookUpTrigger : Requires the user to orient their phone camera upwards before localizing.  

HelloVPS : Example script that demonstrates how to use Sturfee-VPS API calls after localization is complete. Also includes calls to perform Relocalization



*Note* : This scene is designed to run only on Android phone. Running it in editor will lead to errors or crashes.
