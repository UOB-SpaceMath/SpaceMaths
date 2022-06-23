using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (Application.platform == RuntimePlatform.LinuxEditor ||
            Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            GameObject.Find("AR Session Origin").SetActive(false);
            GameObject.Find("AR Session").SetActive(false);
            GameObject.Find("AR Canvas").SetActive(false);
        }
        else
        {
            GameObject.Find("Debug Camera").SetActive(false);
        }
    }
}
