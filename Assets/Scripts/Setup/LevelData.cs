using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "MatchThree")]
public class LevelData : ScriptableObject
{
    public int level;
    public GameObject tilePrefab;
    public GameObject boardPrefab;

}
