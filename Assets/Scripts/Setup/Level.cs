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
    public List<Index2D> selectedTiles;


    private void Start()
    {
        InitializeLevel();
    }

    #region Setup

    private void InitializeLevel()
    {
        selectedTiles = new List<Index2D>();
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
                Index2D index = new Index2D(j, i);
                tile.transform.localPosition = GetTilePosition(index);
                tile.index = index;
                tileMatrix[j, i] = tile;

            }
        }

        Tile.level = this;

        SetupCamera();
    }

    private void SetupCamera()
    {
        //camera setup
        Camera.main.orthographicSize = data.boardWidth+data.emptySpace;
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
        if (HaveValidNeighbours(index))
        {
            //Debug.Log("Valid");
        }
        else
        {
            //Debug.Log("Not valid");

        }
    }
    public void EndLinking()
    {
        if (selectedTiles.Count >= data.minimumMatch)
        {
            DestroyTiles();
            DropTiles();

            //if(!ValidBoard())
            //{
            //    ReshuffleBoard();
            //}
        }
        else
        {
            DeselectTiles();

        }

    }
  
    private void DropTiles()
    {
        for(int i=0;i<selectedTiles.Count;i++)
        {
            StartCoroutine(DropTile(selectedTiles[i]));
        }
    }

    IEnumerator DropTile(Index2D index)
    {
        yield return new WaitUntil(() => tileMatrix[index.x, index.y] == null);
        Drop(index);
    }

    private void Drop(Index2D index)
    {
        //search for firast available in y axis, if there is not make one

        for (int i = 0; i < data.boardHeight; i++)
        {
            if ((index.y + i) < data.boardHeight && tileMatrix[index.x, index.y + i] != null)
            {
                tileMatrix[index.x, index.y + i].transform.localPosition = GetTilePosition(index);
                tileMatrix[index.x, index.y] = tileMatrix[index.x, index.y + i];
                tileMatrix[index.x, index.y + i] = null;
                return;
            }
        }

        //make one

        //generete new ones for not selected

    }

    private void ReshuffleBoard()
    {

    }

    private bool ValidBoard()
    {
        Index2D index = new Index2D(0, 0);

        for(int i=0;i<data.boardHeight;i++)
        {
            for(int j=0;j<data.boardWidth;j++)
            {
                index.x = i;
                index.y = j;
                if (HaveValidMatch(index))
                    return true;
            }
        }

        return false;
    }

    private bool HaveValidMatch(Index2D index)
    {
        //if index have valid neighbour and his neighbour has valid neighbour there is a valid match
        //TODO should be updated to match data.minMatch and not only three

        Index2D neighbour = GetNeighbour(index);
        if (neighbour.Valid() && GetNeighbour(neighbour).Valid())
            return true;

        return false;
    }

    /// <summary>
    /// Check if tile on position index have valid neighbours
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool HaveValidNeighbours(Index2D index)
    {
        bool valid = false;
        for (int i = 0; i < neighboursIndexes.Length; i++)
        {
            int neighbourX = index.x + neighboursIndexes[i].x;
            int neighbourY = index.y + neighboursIndexes[i].y;

            if (neighbourX < 0 || neighbourX >= data.boardWidth || neighbourY < 0 || neighbourY >= data.boardHeight)
                continue;

            if (tileMatrix[index.x, index.y].data == tileMatrix[neighbourX, neighbourY].data)
            {
                //tileMatrix[neighbourX,neighbourY].SelectTile();
                valid = true;
            }
        }

        return valid;
    }

    private Index2D GetNeighbour(Index2D index)
    {
        Index2D indexNeighbour = new Index2D(-1, -1);

        for (int i = 0; i < neighboursIndexes.Length; i++)
        {
            int neighbourX = index.x + neighboursIndexes[i].x;
            int neighbourY = index.y + neighboursIndexes[i].y;

            if (neighbourX < 0 || neighbourX >= data.boardWidth || neighbourY < 0 || neighbourY >= data.boardHeight)
                continue;

            if (tileMatrix[index.x, index.y].data == tileMatrix[neighbourX, neighbourY].data)
            {
                indexNeighbour.x = neighbourX;
                indexNeighbour.y = neighbourY;
                return indexNeighbour;
            }
        }

        return indexNeighbour;

    }

    /// <summary>
    /// Check if tile on position index is valid neighbour to currently selected tiles
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool IsNeighbour(Index2D index)
    {
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            int neighbourX = selectedTiles[i].x;
            int neighbourY = selectedTiles[i].y;

            if (Mathf.Abs(neighbourX - index.x) <= 1 && Mathf.Abs(neighbourY - index.y) <= 1
                && tileMatrix[neighbourX, neighbourY].data == tileMatrix[index.x, index.y].data)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Tiles

    /// <summary>
    /// Deselect all currently selected tiles
    /// </summary>
    public void DeselectTiles()
    {
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            Debug.Log(i);

            tileMatrix[selectedTiles[i].x,selectedTiles[i].y].DeselectTile();
        }

        selectedTiles.Clear();

    }

    /// <summary>
    /// Destroy all currently selected tiles
    /// </summary>
    public void DestroyTiles()
    {
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            tileMatrix[selectedTiles[i].x, selectedTiles[i].y].DeselectTile();
            tileMatrix[selectedTiles[i].x, selectedTiles[i].y].DestroyTileAnimation();
        }

    }

    public void AddTile(int x,int y)
    {
        selectedTiles.Add(new Index2D(x,y));
    }

    private Vector3 GetTilePosition(Index2D index)
    {
        float bottom_x = -data.boardWidth / 2;
        float bottom_y = -data.boardHeight / 2;
        return new Vector3(bottom_x + index.x, bottom_y + index.y, -2);
    }

    #endregion
}
[System.Serializable]
public struct Index2D
{
    public int x;
    public int y;

    public Index2D(int x,int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Valid()
    {
        if (x >= 0 && y >= 0)
            return true;

        return false;
    }

}