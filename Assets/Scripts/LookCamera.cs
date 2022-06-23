using UnityEngine;

public class LookCamera : MonoBehaviour
{
    private GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        // setup camera to use
        if (Application.platform == RuntimePlatform.LinuxEditor ||
           Application.platform == RuntimePlatform.WindowsEditor ||
           Application.platform == RuntimePlatform.OSXEditor)
        {
            Camera = GameObject.Find("Debug Camera");
        }
        else
        {
            Camera = GameObject.Find("AR Session Origin/AR Camera");
        }
        // set camera for canvas
        GetComponentInChildren<Canvas>().worldCamera = Camera.GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Camera.transform.TransformVector(Vector3.forward), Camera.transform.TransformVector(Vector3.up));
    }
}
