using UnityEngine;

public class LookCamera : MonoBehaviour
{
    // Drag the camera to be tracked in the inspector
    [SerializeField]
    private GameObject Camera;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Camera.transform.TransformVector(Vector3.forward), Camera.transform.TransformVector(Vector3.up));
    }
}
