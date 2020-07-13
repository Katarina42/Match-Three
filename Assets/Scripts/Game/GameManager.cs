using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    [Header("Levels:")]
    public LevelData[] levels;
    private int level;

    private bool loaded;
    public bool Loaded
    {
        get { return loaded; }
        set
        {
            loaded = value;
        }
    }

    private bool levelFinished;
    public bool LevelFinished
    {
        get { return levelFinished; }
        set
        {
            if (value)
            {
                level++;
                LoadMenu();
            }

            levelFinished = value;
        }
    }

    public LevelData currentLevelData
    {
        get { return levels[level-1]; }
    }

    private void Awake()
    {
        if(instance!=null && instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        level = 1;
    }

    private void Start()
    {
        LoadMenu();
    }

    public void LoadMenu()
    {
        LoadSceneAsync("Menu",1f);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
        InitializeLevel(level);
        LevelFinished = false;
    }

    public void LoadLevel(int level)
    {
        this.level = level;
        LoadSceneAsync("Game");
        InitializeLevel(level);
        LevelFinished = false;

    }


    public void LoadSceneAsync(string scene,float wait=0)
    {
        Loaded = false;
        StartCoroutine(LoadSceneAsyncCoroutine(scene,wait));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string scene,float wait)
    {
        yield return new WaitForSeconds(wait);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        while(!asyncLoad.isDone)
        {
            yield return null;
        }

        Loaded = true;
    }

    private void InitializeLevel(int index)
    {
        StartCoroutine(InitializeLevelCoroutine(index));
    }

    IEnumerator InitializeLevelCoroutine(int index)
    {
        yield return new WaitWhile(() => !Loaded);
        GameObject levelObj = Instantiate(currentLevelData.levelPrefab);
        levelObj.GetComponent<Level>().data = currentLevelData;
    }
}
