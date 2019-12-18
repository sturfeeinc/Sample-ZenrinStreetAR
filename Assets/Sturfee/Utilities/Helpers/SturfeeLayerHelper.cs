using UnityEngine;

public static class SturfeeLayerHelper
{
    public static int BuildingLayerMask { get { return 1 << LayerMask.NameToLayer(Sturfee.Unity.XR.Core.Constants.SturfeeLayers.Building); } }
    public static int TerrainLayerMask { get { return 1 << LayerMask.NameToLayer(Sturfee.Unity.XR.Core.Constants.SturfeeLayers.Terrain); } }
}
