using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Level goal:")]
    public List<Target> targetTiles;
    public uint maximumMoves;
    public uint targetPoens;


    [Header("Tiles setup:")]

    [Tooltip("Width of a board, at least 3")] public int boardWidth;
    [Tooltip("Height of a board, at least 3")] public int boardHeight;

    [Tooltip("If random is choosed start board will be randomized, else you will choose board setup")] public bool random;
    private GameObject[,] tilesBoard;
    private GameObject[] tiles;


    public void CreateBoard()
    {
        //tiles = new BoardLayout(boardHeight, boardWidth);
        maximumMoves = 5;
    }

}

[System.Serializable]
public struct Target
{
    public TileInfo tile;
    public uint numberOfTiles;
}
