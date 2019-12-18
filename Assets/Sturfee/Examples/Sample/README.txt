Copyright (c) 2018, Sturfee Inc.

Sturfee VPS SDK
version 1.4.4b1

Scene : Sample

This scene is a representation of how an actual testing would look like without having to make a build on a device. Notice the ProviderSet in SturfeeXRSession inspector is set to Editor/Sample Provider Set.

Note : This scene is useful in understanding the localization process. It removes the dependency on using device's sensors for IMU and GPS data and also the camera.

"Sample Provider set" is the provider set that provides with the providers that are used to simulate a real test field.

SampleVideoProvider : Simulates device's back camera by displaying a set of frames stored in an array. 
SampleImuProvider : Provides IMU orientation to XRCamera by reading orientation from data stored in a text file. This file consists of orientation mapped for each frame that is used by SampleVideoProvider 
SampleGpsProvider : Provides GPS readings to XRSession based on data stored in text file.

SampleManager : Keeps frame array and the data(text) file. Also does the mapping between frames and their respective orientation in Data(text) file.

GameObjects :

SturfeeXRSession : Initializes XRSession using the the selected provider set in ProviderSet dropdown. 

LocalizationManager : Handles all aspects of localization, including the UI, calls to the Sturfee VPS server, and event handling.

SturfeeUIManager : Provides helpful UI functionality outside of localization.

SturfeeXRCamera : Applies Pose obtained from XRSession to XRCamera. XRCamera GameObject should have the tag `XRCamera`.

SturfeeXRLight : A GameObject with Light component which gets its intensity and color values based on current sun direction for the location provided in GpsProvider.

LookUpTrigger : Requires the user to orient their phone camera upwards before localizing.  

HelloVPS : Example script that demonstrates how to use Sturfee-VPS API calls after localization is complete. Also includes calls to perform Relocalization

WorldAnchors :  WorldAnchors are added in this scene to understand how a 3d model/asset can be added to a location and then viewed later through app.


*Note* : This scene is designed to run only in Unity Editor. 
