using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName ="Match-Three/Tile")]
public class TileData : ScriptableObject
{
    [Header("Tile:")]
    public int value;
    public Sprite image;
    public Color color=Color.white;

    [Header("Selected animation:")]

    public AnimationData[] scale;
    public AnimationData[] rotation;

    [Header("Destoy animation:")]
    public AnimationData[] DestroyScale;
    public GameObject particlesOnDestroy;
}

[System.Serializable]
public struct AnimationData
{
    public Vector3 target;
    public float duration;
    public AnimationCurve curve;
}