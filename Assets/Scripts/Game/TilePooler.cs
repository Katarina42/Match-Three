using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePooler : MonoBehaviour
{
    public static TilePooler Instance;
    private Queue<GameObject> tilePool;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(int poolSize,GameObject tilePrefab)
    {
        tilePool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tile = Instantiate(tilePrefab, this.transform);
            tile.SetActive(false);
            tilePool.Enqueue(tile);
        }

    }

    public GameObject SpawnTile()
    {
        GameObject tile = tilePool.Dequeue();

        if (tile.activeInHierarchy)
        {
            Debug.LogWarning("Not enough tiles in pool");
            return null;
        }

        tile.SetActive(true);
        return tile;
    }

    public void ReturnTileToPool(GameObject tile)
    {
        tile.transform.parent = this.transform;
        tile.SetActive(false);
        tilePool.Enqueue(tile);
    }
}
