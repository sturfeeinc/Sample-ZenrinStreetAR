//
//  UnityInterface.hpp
//  SturGProcess
//
//  Created by Patrick Metcalfe on 3/20/18.
//  Copyright © 2018 Sturfee Inc. All rights reserved.
//

#ifndef UnityInterface_hpp
#define UnityInterface_hpp

#include <stdio.h>
#include <stdio.h>
#include <array>
#include <cmath>
//#include <experimental/optional>
#include "./PlatformBase.hpp"
#include "../../Shared/SturgProcesser.hpp"

extern "C" struct BuildingReference;

struct BuildingReference {
    int buildingID;
    int vertexStartIndex;
    int vertexEndIndex;
    int faceStartIndex;
    int faceEndIndex;
    
    BuildingReference(int buildingID, int vertexStartIndex, int vertexEndIndex, int faceStartIndex, int faceEndIndex) : buildingID(buildingID), vertexStartIndex(vertexStartIndex), vertexEndIndex(vertexEndIndex), faceStartIndex(faceStartIndex), faceEndIndex(faceEndIndex) {};
};

class SturGProcessingInvocation {
public:
    SturgProcesser processor;
    
    std::vector<float> tileCenter;
    std::vector<BuildingReference> references;
    std::vector<float> vertices;
    std::vector<int> faces;
    int vertexLength = 0;
    int facesLength = 0;
    int buildingLength = 0;
    
    SturGProcessingInvocation() : tileCenter({}), references({}), vertices({}), faces({}) {};
};
static SturGProcessingInvocation currentInvocation;

long cantorEncode2D(long x, long y) {
    return y + ((x + y)*(x + y + 1)) / 2;
}

std::array<long, 2> cantorDecode2D(long encoded) {
    long w = floor((std::sqrt(8*encoded + 1) - 1) / 2);
    long t = (w*w+ w) / 2;
    long y = encoded - t;
    return { (w - y), y };
}

long cantorEncode3D(long x, long y, long z) {
    return cantorEncode2D(cantorEncode2D(x, y), z);
}

std::array<long, 3> cantorDecode3D(long encoded) {
    std::array<long, 2> secondPair = cantorDecode2D(encoded);
    std::array<long, 2> firstPair = cantorDecode2D(secondPair[0]);
    
    return { firstPair[0], firstPair[1],  secondPair[1] };
}

#endif /* UnityInterface_hpp */
