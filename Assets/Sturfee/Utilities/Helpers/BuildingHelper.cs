using UnityEngine;

public static class BuildingHelper
{
    public static string GetBuildingId(GameObject building)
    {
        var buildingId = building.GetComponent<MeshFilter>().mesh.name.Split(' ')[0];
        return buildingId;
    }
}
