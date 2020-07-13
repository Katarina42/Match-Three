using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Match-Three/Level")]
public class LevelData : ScriptableObject
{
    [Header("Level:")]

    public bool randomize;
    [Min(2)] public float emptySpace = 2;
    [Min(3)] public float minimumMatch = 3;
    public TileData[] tilePool;

    [Header("Level board setup:")]

    [Min(1)] public int level=1;
    [NotNull] public GameObject tilePrefab;
    [NotNull] public GameObject levelPrefab;


    [Min(3)] public int boardWidth = 3;
    [Min(3)] public int boardHeight = 3;


    [Header("Level goal:")]
    [Tooltip("Target")] [NotNull] public List<Target> targetTiles;
    [Min(0)] public int maximumMoves;
    [Min(0)] public int targetScore;


    [HideInInspector] public TileData[] boardTiles;

    [System.Serializable]
    public struct Target
    {
        public TileData tile;
        public uint numberOfTiles;
    }

}

