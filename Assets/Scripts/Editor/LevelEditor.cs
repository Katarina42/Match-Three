using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : EditorWindow
{
    public LevelData level;
    public TileData[,] tiles;
    private bool initialized;

    [MenuItem("Window/Level editor")]
    static void Init()
    {
        LevelEditor window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        window.Show();
    }


    private void OnEnable()
    {
        initialized = false;
    }

    public void OnGUI()
    {
        level = ((LevelData)EditorGUILayout.ObjectField(level, typeof(LevelData), false));

        if ( level == null)
        {
            return;
        }


        if (!level.randomize)
        {
            //initialize tile board if there are already stored data , if not make new one
            if (level.boardTiles != null && level.boardTiles.Length == level.boardWidth*level.boardHeight)
            {
                GetLevelBoard();
                initialized = true;
            }
            else if (!initialized)
            {
                tiles = new TileData[level.boardHeight, level.boardWidth];
                initialized = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PrefixLabel("Board setup:");

            CreateBoard();

            if (level.boardTiles != null && GUILayout.Button("Start preview"))
            {
                StartLevelPreview();
            }

            if (level.boardTiles != null && GUILayout.Button("End preview"))
            {
                EndLevelPreview();
            }

        }

    }


    private void SetLevelBoard()
    {
        level.boardTiles = new TileData[level.boardWidth * level.boardHeight];

        for (int i = 0; i < level.boardHeight; i++)
        {
            for (int j = 0; j < level.boardWidth; j++)
            {

                level.boardTiles[j+i*level.boardWidth]= tiles[i, j];
            }

        }

    }

    private void GetLevelBoard()
    {
        tiles = new TileData[level.boardHeight, level.boardWidth];

        for (int i = 0; i < level.boardHeight; i++)
        {
            for (int j = 0; j < level.boardWidth; j++)
            {

                tiles[i, j] = level.boardTiles[j + i * level.boardWidth];
            }

        }

    }

    private void CreateBoard()
    {
        EditorGUILayout.BeginVertical();
        bool assigned = true;

        for (int i = 0; i < level.boardHeight; i++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int j = 0; j < level.boardWidth; j++)
            {
                tiles[i, j] = ((TileData)EditorGUILayout.ObjectField(tiles[i, j], typeof(TileData), false));
                if (tiles[i, j] == null)
                    assigned = false;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndVertical();

        if (assigned)
            SetLevelBoard();

    }

    private void StartLevelPreview()
    {
        GameObject levelObj = Instantiate(level.levelPrefab);

        levelObj.GetComponent<Level>().data = level;

        levelObj.GetComponent<Level>().SetupTiles();
    }



    private void EndLevelPreview()
    {
        GameObject level = GameObject.FindWithTag("Level");

        if (level != null)
            DestroyImmediate(level);
    }

}
