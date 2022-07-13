using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
 public void LoadGame()
 {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
 }


 public void QuitGame()
 {
    Application.Quit();
 }
}
