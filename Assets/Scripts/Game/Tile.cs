using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData data;


    private void OnMouseDown()
    {
        Debug.Log("Clicked tile " + data.name);
    }

}
