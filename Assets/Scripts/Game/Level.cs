using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelData data;
    private const float CAMERA_OFFSET = -0.5F;
    private Tile[,] board;
    private TileIndex[] neighboursIndexes;
    private List<TileIndex> selectedTiles;
    private Stack<TileLink> links;
    private GameObject boardObj;

    private void Awake()
    {
        boardObj = GameObject.FindWithTag("Board");

        links = new Stack<TileLink>();
    }

    private void Start()
    {
        TilePooler.Instance.Initialize(data.poolSize, data.tilePrefab);
        InitializeLevel();
    }

    #region Setup

    private void InitializeLevel()
    {
        selectedTiles = new List<TileIndex>();
        SetupNeighboursIndexes();
        SetupTiles();
    }

    public void SetupTiles()
    {
        if (boardObj == null)
        {
            return;
        }

        board = new Tile[data.boardWidth, data.boardHeight];

        float bottom_x = -data.boardWidth / 2;
        float bottom_y = -data.boardHeight / 2;

        for (int i = 0; i < data.boardHeight; i++)
        {
            for (int j = 0; j < data.boardWidth; j++)
            {
                AddNewTile(new TileIndex(j, i), data.boardTiles[j + i * data.boardWidth]);

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

    }

    private void SetupNeighboursIndexes()
    {
        neighboursIndexes = new TileIndex[8];
        neighboursIndexes[0] = new TileIndex(-1, -1);
        neighboursIndexes[1] = new TileIndex(-1, 0);
        neighboursIndexes[2] = new TileIndex(-1, 1);
        neighboursIndexes[3] = new TileIndex(0, 1);
        neighboursIndexes[4] = new TileIndex(1, 1);
        neighboursIndexes[5] = new TileIndex(1, 0);
        neighboursIndexes[6] = new TileIndex(1, -1);
        neighboursIndexes[7] = new TileIndex(0, -1);
    }

    #endregion

    #region Game

    public bool CheckLinking(TileIndex index)
    {
        if (Tile.lastSelected == null || index==Tile.lastSelected.index)
            return true;


        TileLink link = new TileLink(Tile.lastSelected.index, index);

        if (links.Count > 0)
        {
            TileLink last = links.Peek();

            if ((last.b == link.a && last.a == link.b))
            {
                Tile.lastSelected.Selected = false;
                links.Pop();
                return false;
            }
            else
            {
                links.Push(link);
                return true;
            }
        }
        else if(links.Count==0)
        {
            links.Push(link);
            return true;
        }

        return false;
    }
    public void EndLinking()
    {
        if (selectedTiles.Count >= data.minimumMatch)
        {
            DestroyTiles();
        }
        else
        {
            DeselectTiles();
        }

        links.Clear();
        Tile.lastSelected = null;
    }
    private void ReshuffleBoard()
    {

    }
    private bool ValidBoard()
    {
        TileIndex index = new TileIndex(0, 0);

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
    private bool HaveValidMatch(TileIndex index)
    {
        //if index have valid neighbour and his neighbour has valid neighbour there is a valid match
        //TODO should be updated to match data.minMatch and not only three

        TileIndex neighbour = GetNeighbour(index);
        if (neighbour.Valid() && GetNeighbour(neighbour).Valid())
            return true;

        return false;
    }
    private TileIndex GetNeighbour(TileIndex index)
    {
        TileIndex indexNeighbour = new TileIndex(-1, -1);

        for (int i = 0; i < neighboursIndexes.Length; i++)
        {
            int neighbourX = index.x + neighboursIndexes[i].x;
            int neighbourY = index.y + neighboursIndexes[i].y;

            if (neighbourX < 0 || neighbourX >= data.boardWidth || neighbourY < 0 || neighbourY >= data.boardHeight)
                continue;

            if (board[index.x, index.y].data == board[neighbourX, neighbourY].data)
            {
                indexNeighbour.x = neighbourX;
                indexNeighbour.y = neighbourY;
                return indexNeighbour;
            }
        }

        return indexNeighbour;

    }
    public bool IsNeighbour(TileIndex index)
    {
        if (selectedTiles.Count == 0)
            return true;

        for (int i = 0; i < selectedTiles.Count; i++)
        {
            int neighbourX = selectedTiles[i].x;
            int neighbourY = selectedTiles[i].y;

            if (Mathf.Abs(neighbourX - index.x) <= 1 && Mathf.Abs(neighbourY - index.y) <= 1
                && board[neighbourX, neighbourY].data == board[index.x, index.y].data)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Tiles

    public void DropTiles(TileIndex index)
    {
        if (index == null)
            return;
        
        for (int i = index.y + 1; i < data.boardHeight; i++)
        {
            if (board[index.x, i] != null)
            {
                MoveTile(board[index.x, i], new TileIndex(index.x, i - 1));
            }
        }
        
        
        AddNewTile(new TileIndex(index.x, data.boardHeight - 1));

        if (selectedTiles.Count == 0 && !ValidBoard())
        {
            ReshuffleBoard();
        }
    }

    public void DeselectTiles()
    {
        while(selectedTiles.Count>0)
        {
            board[selectedTiles[0].x, selectedTiles[0].y].Selected = false;
        }

    }
    public void DestroyTiles()
    {
        selectedTiles.Sort((a, b) => b.y.CompareTo(a.y));

        while (selectedTiles.Count > 0)
        {
            int x = selectedTiles[0].x;
            int y = selectedTiles[0].y;
            board[x, y].Selected = false;
            board[x, y].DestroyTileAnimation();

        }

    }
    private void AddNewTile(TileIndex index, TileData tileData = null)
    {
        GameObject tileObj = TilePooler.Instance.SpawnTile();
        tileObj.transform.parent=boardObj.transform;
        tileObj.transform.localPosition = GetTilePosition(index);

        if (tileData == null)
        {
            int random = Random.Range(0, data.tiles.Length);
            tileData = data.tiles[random];
        }

        Tile tile = tileObj.GetComponent<Tile>();
        tile.data = tileData;
        tileObj.GetComponent<SpriteRenderer>().sprite = tileData.image;
        board[index.x, index.y] = tile;
        tile.index = index;
    }
    public void AddTileSelected(TileIndex index)
    {
        selectedTiles.Add(index);
    }
    public void RemoveTileSelected(TileIndex index)
    {
        selectedTiles.Remove(index);
    }
    private void MoveTile(Tile tile, TileIndex targetIndex)
    {
        board[tile.index.x, tile.index.y].transform.localPosition = GetTilePosition(targetIndex);
        board[targetIndex.x, targetIndex.y] = board[tile.index.x, tile.index.y];
        board[tile.index.x, tile.index.y] = null;
        board[targetIndex.x, targetIndex.y].index = targetIndex;

    }
    private Vector3 GetTilePosition(TileIndex index)
    {
        float bottom_x = -data.boardWidth / 2;
        float bottom_y = -data.boardHeight / 2;
        return new Vector3(bottom_x + index.x, bottom_y + index.y, -2);
    }

    #endregion
}
