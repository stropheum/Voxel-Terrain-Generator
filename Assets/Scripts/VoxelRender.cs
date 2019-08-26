using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelRender : MonoBehaviour
{
    [SerializeField] private AtlasData atlasData = null;
    [SerializeField] private Chunk chunkPrefab = null;
    [SerializeField] private float scale = 1.0f;
    [SerializeField] private VoxelData voxelData = null;
    public float Scale
    {
        get => scale;
        set
        {
            scale = value;
            AdjustedScale = value * 0.5f;
        }
    }

    public AtlasData AtlasData => atlasData;
    public float AdjustedScale { get; private set; }
    public Vector3 TerrainOrigin { get; private set; }
    private Chunk[,,] chunks;

    private void Awake()
    {
        Debug.Assert(atlasData != null, $"{gameObject.name}: {nameof(atlasData)} must be set in inspector");
        Debug.Assert(chunkPrefab != null, $"{gameObject.name}: {nameof(chunkPrefab)} must be set in inspector");

        var rend = GetComponent<Renderer>();
        rend.material.mainTexture = atlasData.Texture;
        chunkPrefab.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = AtlasData.Texture;

        voxelData.Generate();
        chunks = new Chunk[voxelData.Width / Chunk.Width, voxelData.Height / Chunk.Height, voxelData.Depth / Chunk.Depth];
        Scale = scale;
        TerrainOrigin = transform.position - new Vector3(
                            (voxelData.Width * scale) / 2.0f,
                            -0.5f, // Always make the bottom cube rest at y = 0
                            (voxelData.Depth * scale) / 2.0f);
    }

    private void Start()
    {
        StartCoroutine(GenerateChunks());
    }

    private IEnumerator GenerateChunks()
    {
        for (int z = 0; z < chunks.GetLength(2); z++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    var chunk = Instantiate(chunkPrefab, transform);
                    chunk.Context = this;
                    chunk.Offset = new Vector3Int(x * Chunk.Width, y * Chunk.Height, z * Chunk.Depth);
                    chunk.transform.position = TerrainOrigin + chunk.Offset;
                    chunks[x, y, z] = chunk;
                    chunks[x, y, z].hideFlags = HideFlags.HideInHierarchy;
                    StartCoroutine(chunk.GenerateVoxelMesh(voxelData));
                    yield return null;
                }
            }
        }

        yield return null;
    }

}
