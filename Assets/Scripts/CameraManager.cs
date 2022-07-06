using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    Camera _debugCamera;
    [SerializeField]
    Camera _normalCamera;

    Camera _currentCamera;
    void Awake()
    {
        if (Application.platform == RuntimePlatform.LinuxEditor ||
            Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            foreach (GameObject canvas in GameObject.FindGameObjectsWithTag("Canvas"))
            {
                canvas.GetComponent<Canvas>().worldCamera = _debugCamera;
            }
            _currentCamera = _debugCamera;
            _normalCamera.gameObject.SetActive(false);
            _debugCamera.gameObject.SetActive(true);
        }
        else
        {
            foreach (GameObject canvas in GameObject.FindGameObjectsWithTag("Canvas"))
            {
                canvas.GetComponent<Canvas>().worldCamera = _normalCamera;
            }
            _currentCamera = _normalCamera;
            _normalCamera.gameObject.SetActive(true);
            _debugCamera.gameObject.SetActive(false);
        }
    }

    public Camera GetCamera()
    {
        return _currentCamera;
    }
}
