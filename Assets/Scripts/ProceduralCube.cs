using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCube : MonoBehaviour
{
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    [SerializeField] private Vector3Int pos;
    [SerializeField] private float scale = 1.0f;
    private float adjustedScale;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        adjustedScale = scale * 0.5f;
    }

    private void Start()
    {
        MakeCube(
            adjustedScale, 
            new Vector3((float)pos.x * scale, (float)pos.y * scale, (float)pos.z * scale));
        UpdateMesh();
    }
     
    private void MakeCube(float cubeScale, Vector3 cubePosition)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            MakeFace(i, cubeScale, cubePosition);
        }
    }

    private void MakeFace(int direction, float faceScale, Vector3 facePosition)
    {
        vertices.AddRange(CubeMeshData.FaceVertices(direction, faceScale, facePosition));

        int vCount = vertices.Count;

        triangles.Add(vCount - 4);
        triangles.Add(vCount - 4 + 1);
        triangles.Add(vCount - 4 + 2);

        triangles.Add(vCount - 4);
        triangles.Add(vCount - 4 + 2);
        triangles.Add(vCount - 4 + 3);
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.Optimize();
    }
}
