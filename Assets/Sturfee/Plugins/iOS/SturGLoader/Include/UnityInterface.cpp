//
//  UnityInterface.cpp
//  SturGProcess
//
//  Created by Patrick Metcalfe on 3/20/18.
//  Copyright © 2018 Sturfee Inc. All rights reserved.
//

#include "UnityInterface.hpp"

#define BUILD_NUMBER 9
extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
getBuildVersion() {
    return BUILD_NUMBER;
}

//void clearUnneededResources();
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
clearUnneededResources() {
    currentInvocation.vertices.clear();
    currentInvocation.references.clear();
    currentInvocation.faces.clear();
    currentInvocation.tileCenter.clear();
}

//long processSturGFileData(System.IntPtr /* byte[] */ sturGDataPoint, long length);
extern "C" long UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
processSturGFileData(char* sturGData, int length) {
    clearUnneededResources();
    SturGModel model = currentInvocation.processor.processFromPointer(sturGData, (size_t)length);
    
    int buildingCount = (int)model.buildings.size();
    
    currentInvocation.vertices.reserve(model.vertexCount * 3);
    currentInvocation.faces.reserve(model.facesCount * 3);
    
    int vertexTally = 0;
    int facesTally = 0;
    
    std::vector<BuildingReference> buildingReferences;
    std::transform(model.buildings.begin(), model.buildings.end(), std::back_inserter(buildingReferences), [&vertexTally, &facesTally](SturGBuilding building) {
        int vertexStart = vertexTally;
        int faceStart = facesTally;
        std::for_each(building.vertices.begin(), building.vertices.end(), [&vertexTally](Point3<float> vertex){
            vertexTally += 3;
            currentInvocation.vertices.push_back(vertex[0]);
            currentInvocation.vertices.push_back(vertex[1]);
            currentInvocation.vertices.push_back(vertex[2]);
        });
        
        std::for_each(building.faces.begin(), building.faces.end(), [&facesTally](Point3<uint16_t> face){
            facesTally += 3;
            currentInvocation.faces.push_back(face[0]);
            currentInvocation.faces.push_back(face[1]);
            currentInvocation.faces.push_back(face[2]);
        });
        return BuildingReference((int)building.descriptor.id, vertexStart, vertexTally, faceStart, facesTally);
    });
    
    currentInvocation.references = buildingReferences;
    currentInvocation.tileCenter = std::vector<float>(model.metadata.tileCenter.begin(), model.metadata.tileCenter.end());
    
    return cantorEncode3D(buildingCount, vertexTally, facesTally);
}

//int getVertexLength();
extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
getVertexLength() {
    return (int)currentInvocation.vertices.size();
}

//int getFacesLength();
extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
getFacesLength() {
    return (int)currentInvocation.faces.size();
}

//int getBuildingsLength();
extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
getBuildingsLength() {
    return (int)currentInvocation.references.size();
}

//void fillTileCenter(out System.IntPtr /* float[] */ tileCenterPointer, long length);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
fillTileCenter(float** tileCenterPointer, int length) {
    *tileCenterPointer = currentInvocation.tileCenter.data();
}

//void fillBuildingReferences(out System.IntPtr /* BuildingReference[] */ buildingReferencePointer, long length);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
fillBuildingReferences(BuildingReference** buildingReferences, int length) {
    *buildingReferences = currentInvocation.references.data();
}

//void fillVertexData(out System.IntPtr /* float[] */ vertexDataPointer, long length);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
fillVertexData(float** vertexDataPointer, int length) {
    *vertexDataPointer = currentInvocation.vertices.data();
}

//void fillFaceData(out System.IntPtr /* int[] */ faceDataPointer, long length);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
fillFaceData(int** faceDataPointer, int length) {
    *faceDataPointer = currentInvocation.faces.data();
}
