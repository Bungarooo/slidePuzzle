using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzle : MonoBehaviour
{
    //https://www.youtube.com/watch?v=X8YnoIq1d1g
    //at 11:22 in video

    public Texture2D image;
    public int blocksPerLine;
    public int shuffleLength = 20;

    Block emptyBlock;
    Block[,] blocks;
    Queue<Block> inputs;
    bool blockIsMoving;
    int shuffleMovesRemaining;


    void Start()
    {
        CreatePuzzle();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartShuffle();
        }
    }

    void CreatePuzzle()
    {
        blocks = new Block[blocksPerLine,blocksPerLine];
        Texture2D[,] imageSlices = ImageSlicer.GetSlices(image, blocksPerLine);

        for(int y = 0; y < blocksPerLine; y++)
        {
            for(int x = 0; x < blocksPerLine; x++)
            {
                GameObject blockObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blockObject.transform.position = -Vector2.one * (blocksPerLine - 1) * .5f + new Vector2(x, y);

                Block block = blockObject.AddComponent<Block>();
                block.OnBlockPressed += PlayerMoveBlockInput;
                block.OnFinishedMoving += OnBlockFinishedMoving;
                block.Init(new Vector2Int(x, y), imageSlices[x, y]);
                blocks[x, y] = block;

                if (y == 0 && x == blocksPerLine - 1)
                {
                    blockObject.SetActive(false);
                    emptyBlock = block;
                }
            }
        }
        Camera.main.orthographicSize = blocksPerLine * .55f;
        inputs = new Queue<Block>();
    }

    void PlayerMoveBlockInput(Block blockToMove)
    {
        inputs.Enqueue(blockToMove);
        MakeNextPlayerMove();
    }

    void MakeNextPlayerMove()
    {
        while (inputs.Count > 0 && !blockIsMoving)
        {
            MoveBlock(inputs.Dequeue());
        }
    }

    void MoveBlock(Block blockToMove)
    {
        if ((blockToMove.coord - emptyBlock.coord).sqrMagnitude == 1)
        {
            Vector2Int targetCoord = emptyBlock.coord;
            emptyBlock.coord = blockToMove.coord;
            blockToMove.coord = targetCoord;

            Vector2 targetPosition = emptyBlock.transform.position;
            emptyBlock.transform.position = blockToMove.transform.position;
            blockToMove.MoveToPosition(targetPosition, .3f);
            blockIsMoving = true;
        }
    }

    void OnBlockFinishedMoving()
    {
        blockIsMoving = false;
        MakeNextPlayerMove();

        if (shuffleMovesRemaining > 0)
        {
            MakeNextShuffleMove();
        }

    }

    void StartShuffle()
    {
        shuffleMovesRemaining = shuffleLength;
        MakeNextShuffleMove();
    }

    void MakeNextShuffleMove()
    {
        Vector2Int[] offsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        int rand = Random.Range(0, offsets.Length);

        for(int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(rand+i)%offsets.Length];
            Vector2Int moveBlockCoord = emptyBlock.coord + offset;

            if (moveBlockCoord.x >= 0 && moveBlockCoord.x < blocksPerLine && moveBlockCoord.y >= 0 && moveBlockCoord.y < blocksPerLine)
            {
                MoveBlock(blocks[moveBlockCoord.x, moveBlockCoord.y]);
                shuffleMovesRemaining--;
                break;
            }
        }
    }
}
