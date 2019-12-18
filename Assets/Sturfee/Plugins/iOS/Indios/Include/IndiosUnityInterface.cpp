//
//  IndiosUnityInterface.cpp
//  Indios
//
//  Created by Patrick Metcalfe on 5/4/17.
//  Copyright © 2017 Sturfee. All rights reserved.
//

/*
 *  NOTE: This file contains very sensitive code.
 *
 *  If a method starts with Unity then it is a method that Unity expects to be
 * defined.
 *  All other methods are customizable but must match up with the C# import
 * definitions.
 *
 *
 *  This files contains the core way the plugin and Unity communicate. It's very
 *  complicated and intertwined.
 *
 */

#include "IndiosUnityInterface.hpp"
#include "Indios.hpp"

#pragma mark - Variables

Indios IndiosInstance;

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API void* CreateInferenceEngine(char* path)
{
    return IndiosInstance.CreateInferenceEngine(path);
}

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API void DeleteInferenceEngine(void* inferenceEngine)
{
    IndiosInstance.DeleteInferenceEngine(inferenceEngine);
}

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API void runInParallel(void* inferenceEngine, void* dataHandle, int width, int height, int channels)
{
    IndiosInstance.runInParallel(inferenceEngine, dataHandle, width, height , channels);
}

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API bool IsDone(void* inferenceEngine)
{
    return IndiosInstance.IsDone(inferenceEngine);
}


extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API int getResult(void* inferenceEngine)
{
    return IndiosInstance.getResult(inferenceEngine);
}

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API void fillProbabilites(void* inferenceEngine, float* indoor, float* outdoor) {
    IndiosInstance.fillProbabilites(inferenceEngine, indoor, outdoor);
}
