using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var debugCam = GameObject.Find("Debug Camera");
        var arCam = GameObject.Find("AR Session Origin/AR Camera");

        if (Application.platform == RuntimePlatform.LinuxEditor ||
            Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            foreach (GameObject canvas in GameObject.FindGameObjectsWithTag("Canvas"))
            {
                canvas.GetComponent<Canvas>().worldCamera = debugCam.GetComponent<Camera>();
                arCam.SetActive(false);
            }
        }
        else
        {
            debugCam.SetActive(false);
        }
    }
}
