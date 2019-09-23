using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(
    typeof(MeshFilter), 
    typeof(MeshRenderer), 
    typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public static int Width => 24;
    public static int Height => 8;
    public static int Depth => 24;
    public VoxelTerrain Context { get; set; } = null;
    public Vector3Int Offset { get; set; } = Vector3Int.zero;
    
    private static Dictionary<Direction, Vector3> NormalMap { get; } = new Dictionary<Direction, Vector3>
    {
        { Direction.North, new Vector3( 0,  0,  1) },
        { Direction.East,  new Vector3( 1,  0,  0) },
        { Direction.South, new Vector3( 0,  0, -1) },
        { Direction.West,  new Vector3(-1,  0,  0) },
        { Direction.Up,    new Vector3( 0,  1,  0) },
        { Direction.Down,  new Vector3( 0, -1,  0) },
    };

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uv;
    private List<int> triangles;
    private MeshCollider meshCollider;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        gameObject.hideFlags |= HideFlags.HideInHierarchy;
    }

    public IEnumerator GenerateVoxelMesh(VoxelData data)
    {
        Debug.Assert(Context != null,
            "Attempting to generate chunk mesh with no context specified");

        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uv = new List<Vector2>();
        triangles = new List<int>();

        for (int z = 0; z < Depth; z++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var index = new Vector3Int(x, y, z);

                    if (data.GetCell(Offset + index) == 0)
                    {
                        continue;
                    }

                    var chunkOrigin = new Vector3(
                                          -Width * 0.5f,
                                          -Height * 0.5f,
                                          -Depth * 0.5f);

                    MakeCube(
                        cubeScale: Context.AdjustedScale,
                        cubePosition: chunkOrigin + new Vector3(
                                          x * Context.Scale,
                                          y * Context.Scale,
                                          z * Context.Scale),
                        index: Offset + index,
                        data: data);
                }
            }
        }

        yield return null;

        UpdateMesh();

        yield return null;
    }

    private void MakeCube(float cubeScale, Vector3 cubePosition, Vector3Int index, VoxelData data)
    {
        for (int i = 0; i < 6; i++)
        {
            if (data.GetNeighbor(index, (Direction)i) == 0)
            {
                MakeFace((Direction)i, cubeScale, cubePosition, (BlockType)data.Grid[index.x, index.y, index.z]);
            }
        }
    }

    private void MakeFace(Direction direction, float faceScale, Vector3 facePosition, BlockType blockType)
    {
        var faceVertices = CubeMeshData.FaceVertices(direction, faceScale, facePosition);
        vertices.AddRange(faceVertices);

        for (int i = 0; i < faceVertices.Length; i++)
        {
            normals.Add(NormalMap[direction]);
        }

        // Handle UV mapping of the new vertices
        var spriteSize = new Vector2(
            1.0f / (float)Context.AtlasData.SpriteCount.x, 
            1.0f / (float)Context.AtlasData.SpriteCount.y);
        var pixel = new Vector2(
            1.0f / Context.AtlasData.AtlasResolution.x,
            1.0f / Context.AtlasData.AtlasResolution.y);
        var buffer = new Vector2(
            pixel.x * Context.AtlasData.Padding, 
            pixel.y * Context.AtlasData.Padding);

        var index = Context.AtlasData.FaceDataMap[blockType].Face[direction];
        var min = new Vector2(index.x * spriteSize.x, index.y * spriteSize.y);
        var max = new Vector2((index.x + 1) * spriteSize.x, (index.y + 1) * spriteSize.y);

        uv.Add(new Vector2(min.x + buffer.x, max.y - buffer.y));  // top-left
        uv.Add(new Vector2(max.x - buffer.x, max.y - buffer.y));  // top-right
        uv.Add(new Vector2(max.x - buffer.x, min.y + buffer.y));  // bot-right
        uv.Add(new Vector2(min.x + buffer.x, min.y + buffer.y));  // bot-left

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
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.Optimize();

        meshCollider.sharedMesh = mesh;
        meshCollider.sharedMesh.MarkDynamic();
    }

}
