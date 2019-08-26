using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CubeMeshData
{
    public static readonly Vector3[] Vertices =
    {
        new Vector3( 1,  1,  1),
        new Vector3(-1,  1,  1),
        new Vector3(-1, -1,  1),
        new Vector3( 1, -1,  1),
        new Vector3(-1,  1, -1),
        new Vector3( 1,  1, -1),
        new Vector3( 1, -1, -1),
        new Vector3(-1, -1, -1),
    };

    public static readonly int[][] FaceTriangles =
    {
        new int[] { 0, 1, 2, 3 }, // North
        new int[] { 5, 0, 3, 6 }, // East
        new int[] { 4, 5, 6, 7 }, // South
        new int[] { 1, 4, 7, 2 }, // West
        new int[] { 5, 4, 1, 0 }, // Up
        new int[] { 3, 2, 7, 6 }, // Down
    };

    public static Vector3[] FaceVertices(int dir, float scale, Vector3 position)
    {
        var fv = new Vector3[4];

        for (int i = 0; i < fv.Length; i++)
        {
            fv[i] = (Vertices[FaceTriangles[dir][i]] * scale) + position;
        }

        return fv;
    }

    public static Vector3[] FaceVertices(Direction dir, float scale, Vector3 position)
    {
        return FaceVertices((int) dir, scale, position);
    }
}
