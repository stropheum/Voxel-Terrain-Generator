using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseClick : MonoBehaviour
{
    [SerializeField] private BlockType selectedBlockType = BlockType.Brick;
    private Vector3? hitPosition = null;
    private Vector3? normal = null;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Assert(Camera.main != null);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity))
            {
                return;
            }

            hitPosition = hit.point;
            normal = hit.normal;

            var chunk = hit.transform.GetComponent<Chunk>();
            if (chunk != null)
            {
                chunk.Context.AddVoxel(hit, selectedBlockType);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Debug.Assert(Camera.main != null);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity))
            {
                return;
            }

            hitPosition = hit.point;
            normal = hit.normal;

            var chunk = hit.transform.GetComponent<Chunk>();
            if (chunk != null)
            {
                chunk.Context.RemoveVoxel(hit);
            }
        }
    }

    public void SelectGrass()
    {
        selectedBlockType = BlockType.Grass;
    }

    public void SelectStone()
    {
        selectedBlockType = BlockType.Stone;
    }

    public void SelectCobble()
    {
        selectedBlockType = BlockType.Cobble;
    }

    public void SelectSand()
    {
        selectedBlockType = BlockType.Sand;
    }

    public void SelectBrick()
    {
        selectedBlockType = BlockType.Brick;
    }

    public void SelectDirt()
    {
        selectedBlockType = BlockType.Dirt;
    }
}
