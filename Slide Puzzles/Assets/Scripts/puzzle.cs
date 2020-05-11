using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzle : MonoBehaviour
{
    public int blocksPerLine;

    void CreatePuzzle()
    {
        for(int y = 0; y < blocksPerLine; y++)
        {
            for(int x = 0; x < blocksPerLine; x++)
            {
                GameObject blockObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blockObject.transform.position = -Vector2.one * (blocksPerLine - 1) * .5f + new Vector2(x, y);
            }
        }
        Camera.main.orthographicSize = blocksPerLine * .55f;
    }

    void Start()
    {
        CreatePuzzle();
    }
}
