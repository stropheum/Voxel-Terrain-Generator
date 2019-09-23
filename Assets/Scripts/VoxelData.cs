using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Terrain/Voxel Data")]
public class VoxelData : ScriptableObject
{
    [SerializeField] private Vector3Int chunkDimensions = Vector3Int.zero;
    [SerializeField] private float seed = 0.0f;
    [SerializeField] private float noiseZoom = 1.0f;
    public int[,,] Grid { get; set; }
    public int Width => Grid.GetLength(0);
    public int Height => Grid.GetLength(1);
    public int Depth => Grid.GetLength(2);

    public void Generate()
    {
        GenerateGrid(new Vector3Int(
            chunkDimensions.x * Chunk.Width,
            chunkDimensions.y * Chunk.Height,
            chunkDimensions.z * Chunk.Depth));
    }

    /// <summary>
    /// Returns a cell with the given coordinates from the absolute origin
    /// </summary>
    /// <param name="index">The index of the cell</param>
    /// <returns>The numerical value of the voxel at the given index</returns>
    public int GetCell(Vector3Int index)
    {
        if (index.x < 0 || index.x >= Width ||
            index.y < 0 || index.y >= Height ||
            index.z < 0 || index.z >= Depth)
        {
            return 0; // Accessing out of grid bounds
        }
        else
        {
            return Grid[index.x, index.y, index.z];
        }
    }

    /// <summary>
    /// Gets the value of the neighbor cell at the specified index in the specified direction
    /// </summary>
    /// <param name="index"> The index of the cell </param>
    /// <param name="dir"> The direction of the neighbor </param>
    /// <returns> The numerical representation of the block type, 0 if it doesn't exist </returns>
    public int GetNeighbor(Vector3Int index, Direction dir)
    {
        var offsetToCheck = offsets[(int)dir];
        var neighbor = index + offsetToCheck;

        if (neighbor.x < 0 || neighbor.x >= Width ||
            neighbor.y < 0 || neighbor.y >= Height ||
            neighbor.z < 0 || neighbor.z >= Depth)
        {
            return 0;
        }

        return GetCell(neighbor);
    }

    /// <summary>
    /// Generates a grid based on a 3-dimensional voxel size
    /// </summary>
    /// <param name="size">The number of voxels in every dimension</param>
    private void GenerateGrid(Vector3Int size)
    {
        Grid = new int[size.x, size.y, size.z];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    var height = 
                        (int) (Mathf.PerlinNoise((seed + x) / noiseZoom, (seed + z) / noiseZoom) * 
                               Grid.GetLength(1));
                    
                    if (y > height)
                    {
                        Grid[x, y, z] = 0;
                    }
                    else if (y < 4)
                    {
                        Grid[x, y, z] = (int) BlockType.Stone;
                    }
                    else if (y == height)
                    {
                        Grid[x, y, z] = (int) BlockType.Grass;
                    }
                    else if (y < height || y < 6)
                    {
                        Grid[x, y, z] = (int) BlockType.Dirt;
                    }
                    else
                    {
                        Grid[x, y, z] = Random.Range(1, 6);
                    }
                }
            }
        }
    }

    private readonly Vector3Int[] offsets =
    {
        new Vector3Int( 0,  0,  1),   // North
        new Vector3Int( 1,  0,  0),   // East
        new Vector3Int( 0,  0, -1),   // South
        new Vector3Int(-1,  0,  0),   // West
        new Vector3Int( 0,  1,  0),   // Up
        new Vector3Int( 0, -1,  0),   // Down
    };
}

public enum Direction
{
    North = 0,
    East  = 1,
    South = 2,
    West  = 3,
    Up    = 4,
    Down  = 5
}

public enum BlockType
{
    Grass  = 1,
    Stone  = 2,
    Cobble = 3,
    Sand   = 4,
    Brick  = 5,
    Dirt   = 6,
}