using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Level setup:")]
    public LevelData data;
    private const float CAMERA_OFFSET= -0.5F;
    private Tile [,] tileMatrix;
    private Index2D[] neighboursIndexes;
    public List<Tile> selectedTiles;

    private void Start()
    {
        InitializeLevel();
    }

    #region Setup

    private void InitializeLevel()
    {
        selectedTiles = new List<Tile>();
        SetupNeighboursIndexes();
        SetupTiles();
    }

    public void SetupTiles()
    {
        GameObject board = GameObject.FindWithTag("Board");

        if (board == null)
        {
            return;
        }

        for (int i = 0; i < data.boardTiles.Length; i++)
        {
            GameObject tile = Instantiate(data.tilePrefab, board.transform);
            tile.GetComponent<Tile>().data = data.boardTiles[i];
            tile.GetComponent<SpriteRenderer>().sprite = data.boardTiles[i].image;
        }

        tileMatrix = new Tile[data.boardHeight, data.boardWidth];
        float bottom_x = -data.boardWidth / 2;
        float bottom_y = -data.boardHeight / 2;

        for (int i = 0; i < data.boardHeight; i++)
        {
            for (int j = 0; j < data.boardWidth; j++)
            {
                Tile tile = board.transform.GetChild(j + i * data.boardWidth).gameObject.GetComponent<Tile>();
                Vector3 target = new Vector3(bottom_x + j, bottom_y + i, tile.transform.localPosition.z);
                tile.transform.localPosition = target;
                tile.level = this;
                tile.index = new Index2D(i, j);
                tileMatrix[i, j] = tile;

            }
        }

        SetupCamera();
    }

    private void SetupCamera()
    {
        //camera setup
        Camera.main.orthographicSize = data.boardWidth;
        Vector3 pos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(CAMERA_OFFSET + (float)data.boardWidth % 2 / 2, pos.y, pos.z);

        //mask setup
        if (Camera.main.transform.childCount != 0)
        {
            Camera.main.transform.GetChild(0).localScale = new Vector3(data.boardWidth, data.boardHeight, 1);
        }
        else
        {
#if UNITY_EDITOR

            Debug.LogWarning("Go to Game scene for preview");

#elif UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID

            Debug.LogWarning("Mask missing as a child of main camera");
#endif
        }
    }

    private void SetupNeighboursIndexes()
    {
        neighboursIndexes = new Index2D[8];
        neighboursIndexes[0] = new Index2D(-1, -1);
        neighboursIndexes[1] = new Index2D(-1, 0);
        neighboursIndexes[2] = new Index2D(-1, 1);
        neighboursIndexes[3] = new Index2D(0, 1);
        neighboursIndexes[4] = new Index2D(1, 1);
        neighboursIndexes[5] = new Index2D(1, 0);
        neighboursIndexes[6] = new Index2D(1, -1);
        neighboursIndexes[7] = new Index2D(0, -1);
    }
#endregion

#region Game

    public void StartLinking(Index2D index)
    {
        if(HaveValidNeighbours(index))
        {
            //Debug.Log("Valid");
        }
        else
        {
            //Debug.Log("Not valid");

        }
    }

    private bool HaveValidNeighbours(Index2D index)
    {
        bool valid = false;
        for(int i=0;i<neighboursIndexes.Length;i++)
        {
            int neighbourX = index.x + neighboursIndexes[i].x;
            int neighbourY = index.y + neighboursIndexes[i].y;

            if (neighbourX < 0 || neighbourX >= data.boardWidth || neighbourY < 0 || neighbourY >= data.boardHeight)
                continue;

            if (tileMatrix[index.x,index.y].data==tileMatrix[neighbourX,neighbourY].data)
            {
                //tileMatrix[neighbourX,neighbourY].SelectTile();
                valid = true;
            }
        }

        return valid;
    }

    public bool IsNeighbour(Index2D index)
    {
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            if (Mathf.Abs(selectedTiles[i].index.x - index.x) <= 1
                && Mathf.Abs(selectedTiles[i].index.y - index.y) <= 1
                && selectedTiles[i].data == tileMatrix[index.x, index.y].data)
            {
                //Debug.Log("Data "+i+" " + selectedTiles[i].data + "\nData "+index.x+" "+index.y+" " + tileMatrix[index.x, index.y].data);
                return true;
            }
        }

        //Debug.Log("Not neighbour");
        return false;
    }

    public void DeselectTiles()
    {
        for (int i=0;i<selectedTiles.Count;i++)
        {
            Debug.Log(i);

            selectedTiles[i].DeselectTile();
        }

        selectedTiles.Clear();

    }

    public void DestroyTiles()
    {
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            selectedTiles[i].DeselectTile();
            selectedTiles[i].DestroyTile();
        }

        selectedTiles.Clear();

    }

    #endregion
}

public struct Index2D
{
    public int x;
    public int y;

    public Index2D(int x,int y)
    {
        this.x = x;
        this.y = y;
    }
}