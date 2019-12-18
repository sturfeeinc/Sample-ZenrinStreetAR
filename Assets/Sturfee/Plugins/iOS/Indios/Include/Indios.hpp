//
//  Indios.hpp
//  Indios
//
//  Created by Patrick Metcalfe on 7/24/18.
//  Copyright © 2018 Sturfee Inc. All rights reserved.
//

#ifndef Indios_
#define Indios_

#include <string>


/* The classes below are exported */
#pragma GCC visibility push(default)

class Indios {
    public:
    void* CreateInferenceEngine(char* path);
    void DeleteInferenceEngine(void* inferenceEngine);
    void runInParallel(void* inferenceEngine, void* dataHandle, int width, int height, int channels);
    bool IsDone(void* inferenceEngine);
    int getResult(void* inferenceEngine);
    void fillProbabilites(void* inferenceEngine, float* indoor, float* outdoor);
};

#pragma GCC visibility pop
#endif
