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


    private void Awake()
    {
        if(instance!=null && instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        level = 0;
    }

    private void Start()
    {
        LoadMenu();
    }

    private void LoadMenu()
    {
        LoadSceneAsync("Menu",1);
        level++;
    }

    public void LoadGame()
    {
        LoadSceneAsync("Game");
        InitializeLevel(level);
    }

    public void LoadLevel(int level)
    {
        LoadSceneAsync("Game");
        InitializeLevel(level);
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
        yield return new WaitUntil(() => Loaded);
        LevelData level = levels[index - 1];
        GameObject levelObj = Instantiate(level.levelPrefab);
        levelObj.GetComponent<Level>().data = level;

    }
}
