using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelTerrain : MonoBehaviour
{
    [SerializeField] private AtlasData atlasData = null;
    [SerializeField] private VoxelData voxelData = null;
    [SerializeField] private Chunk chunkPrefab = null;
    [SerializeField] private float scale = 1.0f;

    public AtlasData AtlasData => atlasData;

    public float Scale
    {
        get => scale;
        set
        {
            scale = value;
            AdjustedScale = value * 0.5f;
        }
    }

    public Vector3 TerrainOrigin =>
        transform.position - 
        new Vector3(
            (voxelData.Width * scale) / 2.0f, 
            (voxelData.Height * scale) / 2.0f,
            (voxelData.Depth * scale) / 2.0f);

    public float AdjustedScale { get; private set; }

    private Chunk[,,] chunks;

    private void Awake()
    {
        Debug.Assert(atlasData != null, 
            $"{gameObject.name}: {nameof(atlasData)} must be set in inspector");
        Debug.Assert(chunkPrefab != null, 
            $"{gameObject.name}: {nameof(chunkPrefab)} must be set in inspector");

        var rend = GetComponent<Renderer>();
        rend.material.mainTexture = atlasData.Texture;
        chunkPrefab.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = AtlasData.Texture;

        voxelData.Generate();
        chunks = new Chunk[voxelData.Width / Chunk.Width, voxelData.Height / Chunk.Height, voxelData.Depth / Chunk.Depth];
        Scale = scale;
    }

    private void Start()
    {
        StartCoroutine(GenerateChunks());
    }

    public void AddVoxel(RaycastHit hit, BlockType blockType)
    {
        var voxelIndex = WorldToGridPosition(hit.point + (hit.normal / 2.0f));
        var chunk = GetChunkAtVoxelIndex(voxelIndex);

        voxelData.Grid[voxelIndex.x, voxelIndex.y, voxelIndex.z] = (int)blockType;
        StartCoroutine(chunk.GenerateVoxelMesh(voxelData));

        // TODO: Actually check if we're bordering a chunk seem rather than doing this naively
        var index = IndexOf(chunk, out var found);
        if (index.x > 0)
        {
            StartCoroutine(chunks[index.x - 1, index.y, index.z].GenerateVoxelMesh(voxelData));
        }

        if (index.x < chunks.GetLength(0) - 1)
        {
            StartCoroutine(chunks[index.x + 1, index.y, index.z].GenerateVoxelMesh(voxelData));
        }

        if (index.y > 0)
        {
            StartCoroutine(chunks[index.x, index.y - 1, index.z].GenerateVoxelMesh(voxelData));
        }

        if (index.y < chunks.GetLength(1) - 1)
        {
            StartCoroutine(chunks[index.x, index.y + 1, index.z].GenerateVoxelMesh(voxelData));
        }

        if (index.z > 0)
        {
            StartCoroutine(chunks[index.x, index.y, index.z - 1].GenerateVoxelMesh(voxelData));
        }

        if (index.z < chunks.GetLength(2) - 1)
        {
            StartCoroutine(chunks[index.x, index.y, index.z + 1].GenerateVoxelMesh(voxelData));
        }
    }

    public void RemoveVoxel(RaycastHit hit)
    {
        var voxelIndex = WorldToGridPosition(hit.point - (hit.normal / 2.0f));
        var chunk = GetChunkAtVoxelIndex(voxelIndex);

        voxelData.Grid[voxelIndex.x, voxelIndex.y, voxelIndex.z] = 0;
        StartCoroutine(chunk.GenerateVoxelMesh(voxelData));

        // TODO: Actually check if we're bordering a chunk seem rather than doing this naively
        var index = IndexOf(chunk, out var found);
        if (found)
        {
            if (index.x > 0)
            {
                StartCoroutine(chunks[index.x - 1, index.y, index.z].GenerateVoxelMesh(voxelData));
            }

            if (index.x < chunks.GetLength(0) - 1)
            {
                StartCoroutine(chunks[index.x + 1, index.y, index.z].GenerateVoxelMesh(voxelData));
            }

            if (index.y > 0)
            {
                StartCoroutine(chunks[index.x, index.y - 1, index.z].GenerateVoxelMesh(voxelData));
            }

            if (index.y < chunks.GetLength(1) - 1)
            {
                StartCoroutine(chunks[index.x, index.y + 1, index.z].GenerateVoxelMesh(voxelData));
            }

            if (index.z > 0)
            {
                StartCoroutine(chunks[index.x, index.y, index.z - 1].GenerateVoxelMesh(voxelData));
            }

            if (index.z < chunks.GetLength(2) - 1)
            {
                StartCoroutine(chunks[index.x, index.y, index.z + 1].GenerateVoxelMesh(voxelData));
            }
        }
    }

    private IEnumerator GenerateChunks()
    {
        var origin = TerrainOrigin + new Vector3(
                         Chunk.Width / 2.0f, 
                         Chunk.Height / 2.0f, 
                         Chunk.Depth / 2.0f);

        for (int z = 0; z < chunks.GetLength(2); z++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    var chunk = Instantiate(chunkPrefab, transform);
                    chunk.Context = this;
                    chunk.Offset = new Vector3Int(
                        x * Chunk.Width, 
                        y * Chunk.Height, 
                        z * Chunk.Depth);
                    chunk.transform.position = origin + chunk.Offset;
                    chunks[x, y, z] = chunk;
                    StartCoroutine(chunk.GenerateVoxelMesh(voxelData));
                    yield return null;
                }
            }
        }

        yield return null;
    }

    public Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        var halfCubeOffset = new Vector3(0.5f, 0.5f, 0.5f);
        var indexPosition = 
            transform.InverseTransformPoint(worldPosition) - 
            TerrainOrigin + 
            halfCubeOffset;

        return Vector3Int.FloorToInt(indexPosition);
    }

    public Vector3Int IndexOf(Chunk chunk, out bool found)
    {
        var result = Vector3Int.zero;
        found = false;

        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    if (chunk == chunks[x, y, z])
                    {
                        result = new Vector3Int(x, y, z);
                        found = true;
                        break;
                    }
                }
            }
        }

        return result;
    }

    public Chunk GetChunkAtVoxelIndex(Vector3Int voxelIndex)
    {
        Debug.Assert(
            voxelIndex.x >= 0 && voxelIndex.x < voxelData.Width &&
                     voxelIndex.y >= 0 && voxelIndex.y < voxelData.Height &&
                     voxelIndex.z >= 0 && voxelIndex.z < voxelData.Depth);

        return chunks[(int)(voxelIndex.x / Chunk.Width), (int)(voxelIndex.y / Chunk.Height), (int)(voxelIndex.z / Chunk.Depth)];
    }

}
