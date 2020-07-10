using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager instance;
    public GameManager Instance
    {
        get { return instance; }
    }

    [Header("Levels:")]
    public List<LevelData> levels;
    private int level;


    private void Awake()
    {
        if(instance!=null && instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }
}
