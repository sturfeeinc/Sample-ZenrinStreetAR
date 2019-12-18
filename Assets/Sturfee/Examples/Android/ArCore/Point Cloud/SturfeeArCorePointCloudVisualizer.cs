using UnityEngine;

using GoogleARCore;

public class SturfeeArCorePointCloudVisualizer : MonoBehaviour {

    private const int MAXPOINTCOUNT = 61440;

    private Mesh _mesh;

    private Vector3[] _points = new Vector3[MAXPOINTCOUNT];

    public void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _mesh.Clear();
    }

    public void Update()
    {
        // Fill in the data to draw the point cloud.
        if (Frame.PointCloud.IsUpdatedThisFrame)
        {
            // Copy the point cloud points for mesh verticies.
            for (int i = 0; i < Frame.PointCloud.PointCount; i++)
            {
                // Updates points from arcore to sturfee so that it can be visible from XrCamera
                _points[i] = TrackingUtils.TrackingPosToSturfeePos(Frame.PointCloud.GetPointAsStruct(i));
            }
            
            // Update the mesh indicies array.
            int[] indices = new int[Frame.PointCloud.PointCount];
            for (int i = 0; i < Frame.PointCloud.PointCount; i++)
            {
                indices[i] = i;
            }

            _mesh.Clear();
            _mesh.vertices = _points;
            _mesh.SetIndices(indices, MeshTopology.Points, 0);
        }
    }
}