//
//  SturgProcesser.hpp
//  SturgProcess
//
//  Created by Patrick Metcalfe on 3/15/18.
//  Copyright © 2018 Sturfee Inc. All rights reserved.
//

#ifndef SturgProcesser_
#define SturgProcesser_

/* The classes below are exported */
#pragma GCC visibility push(default)

#include <iostream>
#include <fstream>
#include <stdio.h>
#include <sstream>
#include <array>
#include <vector>
#include <algorithm>
#include <iostream>     // std::cout, std::fixed
#include <iomanip>      // std::setprecision
#include "SFLogger.hpp"
#include "SFDataStream.hpp"

template <typename T>
/**
 @discussion Android NDK does not have to_string built in
 @return string representation of `value`
 */
std::string sturfee_to_string(T value) {
    std::ostringstream os;
    os << value;
    return os.str();
}

template <typename T>
/**
 @discussion Android NDK does not have to_string built in
 @return string representation of `value`
 */
std::string sturfee_to_string(T value, int precision) {
    std::ostringstream os;
    os.precision(precision);
    os << value;
    return os.str();
}

template<class T>
using Point3 = std::array<T, 3>;

struct BuildingDescriptor {
    uint64_t id;
    uint32_t verticesByteLength;
    uint32_t facesByteLength;
    bool isUInt16;
    bool isTerrain;
    
    BuildingDescriptor(): id(0), verticesByteLength(0), facesByteLength(0), isUInt16(false), isTerrain(false) {};
};

struct SturGBuilding {
    BuildingDescriptor descriptor;
    std::vector<Point3<uint16_t>> faces;
    std::vector<Point3<float>> vertices;
    
    SturGBuilding(BuildingDescriptor descriptor, std::vector<Point3<uint16_t>> faces, std::vector<Point3<float>> vertices) : descriptor(descriptor), faces(faces), vertices(vertices) {};
    SturGBuilding(): descriptor(BuildingDescriptor()), faces({}), vertices({}) {};
};

struct SturGFileMetadata {
    uint32_t version;
    uint32_t length;
    uint32_t modelCount;
    std::array<uint32_t, 3> tileCenter;
};

struct SturGModel {
    SturGFileMetadata metadata;
    std::vector<SturGBuilding> buildings;
    size_t vertexCount;
    size_t facesCount;
    
    SturGModel(SturGFileMetadata metadata, std::vector<SturGBuilding> buildings, size_t vertexCount, size_t facesCount) : metadata(metadata), buildings(buildings), vertexCount(vertexCount), facesCount(facesCount) {};
};

class SturgProcesser
{
    public:
    SturgProcesser();
    
    SturGModel process(std::istream& sturgFileData);
    SturGModel processFromPointer(char* dataHandle, size_t length);
    
    
    private:
    SFLogger logger = SFLogger("SturGProcessor");
};

#pragma GCC visibility pop
#endif
