using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    private Level level;
    public TileData[,] tiles;
    private bool initialized;

    private void OnEnable()
    {
        initialized = false;
    }

    public override void OnInspectorGUI()
    {
       
        EditorStyles.boldLabel.normal.textColor = Color.magenta;
        EditorStyles.boldLabel.fontSize = 13;

        base.OnInspectorGUI();
        level = (Level)target;
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 13;
        titleStyle.normal.textColor= Color.magenta;

     


        if (!level.random && level.boardWidth>=3 && level.boardHeight>=3)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PrefixLabel("Board setup:", EditorStyles.label, titleStyle);
            EditorGUILayout.BeginVertical();

            if (!initialized)
            {
                tiles = new TileData[level.boardHeight, level.boardWidth];
                initialized = true;
            }

            for (int i = 0; i < level.boardHeight; i++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < level.boardWidth; j++)
                {
                    tiles[i,j]=( (TileData)EditorGUILayout.ObjectField(tiles[i,j], typeof(TileData), false));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();
        }

        
        
    }

  
   
}
