using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceMath;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()
    {
        GlobalInformation.CurrentLevelIndex = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}