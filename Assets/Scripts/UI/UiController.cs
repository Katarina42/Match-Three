using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{

    public void LoadLevel(int i)
    {
        GameManager.Instance.LoadLevel(i);
    }

    public void LoadGame()
    {
        GameManager.Instance.LoadGame();
    }

    public void LoadMenu()
    {
        GameManager.Instance.LoadMenu();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
