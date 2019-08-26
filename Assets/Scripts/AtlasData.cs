using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Atlas Data")]
public class AtlasData : ScriptableObject
{
    [SerializeField] private Texture2D texture = null;
    [SerializeField] private Vector2Int spriteResolution = Vector2Int.zero;
    [SerializeField] private Vector2Int spriteCount = Vector2Int.zero;
    [SerializeField] private int padding = 0;

    public Texture2D Texture => texture;
    public Vector2Int SpriteResolution => spriteResolution;
    public Vector2Int SpriteCount => spriteCount;
    public Vector2Int AtlasResolution => (spriteResolution + new Vector2Int(padding * 2, padding * 2)) * spriteCount;
    public int Padding => padding;

    public struct FaceData
    {
        public Dictionary<Direction, Vector2Int> Face { get; }

        public FaceData(
            Vector2Int north,
            Vector2Int east,
            Vector2Int south,
            Vector2Int west,
            Vector2Int up,
            Vector2Int down)
        {
            Face = new Dictionary<Direction, Vector2Int>
            {
                [Direction.North] = north,
                [Direction.East]  = east,
                [Direction.South] = south,
                [Direction.West]  = west,
                [Direction.Up]    = up,
                [Direction.Down]  = down
            };
        }
    }

    public Dictionary<BlockType, FaceData> FaceDataMap { get; } = new Dictionary<BlockType, FaceData>
    {
        {
            BlockType.Dirt, new FaceData(
                new Vector2Int(2, 15), 
                new Vector2Int(2, 15), 
                new Vector2Int(2, 15),
                new Vector2Int(2, 15), 
                new Vector2Int(2, 15), 
                new Vector2Int(2, 15))
        },
        {
            BlockType.Grass, new FaceData(
                new Vector2Int(3, 15),
                new Vector2Int(3, 15),
                new Vector2Int(3, 15),
                new Vector2Int(3, 15),
                new Vector2Int(0, 15),
                new Vector2Int(2, 15))
        },
        {
            BlockType.Stone, new FaceData(
                new Vector2Int(1, 15),
                new Vector2Int(1, 15),
                new Vector2Int(1, 15),
                new Vector2Int(1, 15),
                new Vector2Int(1, 15),
                new Vector2Int(1, 15))
        },
        {
            BlockType.Cobble, new FaceData(
                new Vector2Int(0, 14),
                new Vector2Int(0, 14),
                new Vector2Int(0, 14),
                new Vector2Int(0, 14),
                new Vector2Int(0, 14),
                new Vector2Int(0, 14))
        },
        {
            BlockType.Sand, new FaceData(
                new Vector2Int(2, 14),
                new Vector2Int(2, 14),
                new Vector2Int(2, 14),
                new Vector2Int(2, 14),
                new Vector2Int(2, 14),
                new Vector2Int(2, 14))
        },
        {
            BlockType.Brick, new FaceData(
                new Vector2Int(7, 15),
                new Vector2Int(7, 15),
                new Vector2Int(7, 15),
                new Vector2Int(7, 15),
                new Vector2Int(7, 15),
                new Vector2Int(7, 15))
        },
    };

}
