using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UiGameController :MonoBehaviour
{
    public GameObject targetPrefab;
    public GameObject targetObj;

    private static Dictionary<TileData, Text> targets;

    private void Start()
    {
        if (targetObj != null && targetPrefab != null)
            SetTargetsUi(GameManager.Instance.currentLevelData);
    }


    public void LoadMenu()
    {
        GameManager.Instance.LoadSceneAsync("Menu");
    }

    private void SetTargetsUi(LevelData data)
    {
        targets = new Dictionary<TileData, Text>();
        for(int i=0;i<data.targetTiles.Length;i++)
        {
            GameObject target = Instantiate(targetPrefab, targetObj.transform);
            target.GetComponent<Image>().sprite = data.targetTiles[i].tile.image;
            var text = target.GetComponentInChildren<Text>();
            text.text = data.targetTiles[i].numberOfTiles.ToString();
            targets.Add(data.targetTiles[i].tile, text);
        }
    }

    public static void UpdateTargets(TileData data)
    {
        Text text;
        int res;

        if(targets.TryGetValue(data,out text) && int.TryParse(text.text,out res))
        {
            res--;
            text.text = res.ToString();

            if (res == 0)
            {
                text.color = Color.grey;
                targets.Remove(data);
            }


        }

        if (targets.Count == 0)
            GameManager.Instance.LevelFinished = true;
    }
}
