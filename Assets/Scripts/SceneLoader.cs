using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceMath;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame(int index)
    {
        GlobalInformation.CurrentLevelIndex = 0;
        SceneManager.LoadScene(index);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}