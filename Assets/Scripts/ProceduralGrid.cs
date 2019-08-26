using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralGrid : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [SerializeField] private float cellSize = 1;
    private readonly Vector3 gridOffset = Vector3.zero;
    private readonly Vector2Int gridSize;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Start()
    {
        MakeDiscreteProceduralGrid();
        UpdateMesh();
    }

    private void MakeDiscreteProceduralGrid()
    {
        // Set array sizes
        vertices = new Vector3[gridSize.x * gridSize.y * 4];
        triangles = new int[gridSize.x * gridSize.y * 6];

        // Set tracker integers
        int v = 0;
        int t = 0;

        // Set vertex offset
        float vertexOffset = cellSize * 0.5f;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 cellOffset = new Vector3(x * cellSize, 0, y * cellSize);

                // Populate the vertex and triangle arrays
                vertices[v    ] = new Vector3(-vertexOffset, 0, -vertexOffset) + cellOffset + gridOffset;
                vertices[v + 1] = new Vector3(-vertexOffset, 0,  vertexOffset) + cellOffset + gridOffset;
                vertices[v + 2] = new Vector3( vertexOffset, 0, -vertexOffset) + cellOffset + gridOffset;
                vertices[v + 3] = new Vector3( vertexOffset, 0,  vertexOffset) + cellOffset + gridOffset;

                triangles[t    ] = v;
                triangles[t + 1] = triangles[t + 4] = v + 1;
                triangles[t + 2] = triangles[t + 3] = v + 2;
                triangles[t + 5] = v + 3;

                v += 4; // Iterate to next quad
                t += 6; // Iterate to next 6 vertices of the new quad
            }
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.Optimize();
    }
}
