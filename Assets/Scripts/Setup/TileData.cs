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
    public Sprite backgroundImage;
    public Color color=Color.white;

    [Header("Selected animation:")]

    [Tooltip("Animation will circulate through all animations in array, if you left zero sized there will be no scale animation when tile is selected")]
    public AnimationData[] scale;
    [Tooltip("Animation will circulate through all animations in array, if you left zero sized there will be no rotate animation when tile is selected")]
    public AnimationData[] rotation;

    [Header("Destoy animation:")]
    public float destroyDelay;
    public AnimationData[] destroyScale;
    public GameObject destroyParticles;
    public Color particlesColor=Color.white;
    public AnimationCurve particlesCurve;


}

[System.Serializable]
public struct AnimationData
{
    public Vector3 target;
    public float duration;
    public AnimationCurve curve;
}