//
//  IndiosUnityInterface.hpp
//  Indios
//
//  Created by Patrick Metcalfe on 6/8/17.
//  Copyright © 2018 Sturfee. All rights reserved.
//

#ifndef UnityInterface_hpp
#define UnityInterface_hpp

//#include <experimental/optional>

#include "PlatformBase.hpp"

struct IndiosUnityInterface {
    
};

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API void* CreateInferenceEngine(char* path);

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API void DeleteInferenceEngine(void* inferenceEngine);

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API void runInParallel(void* inferenceEngine, void* dataHandle, int width, int height, int channels);

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API bool IsDone(void* inferenceEngine);

extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API int getResult(void* inferenceEngine);


extern "C" UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API void fillProbabilites(void* inferenceEngine, float* indoor, float* outdoor);
#endif /* UnityInterface_hpp */
