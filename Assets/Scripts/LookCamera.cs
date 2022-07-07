using UnityEngine;

public class LookCamera : MonoBehaviour
{
    [SerializeField]
    private CameraManager _cm;

    void LateUpdate()
    {
        Camera camera;
        if (_cm != null)
        {
            camera = _cm.GetCamera();
        }
        else
        {
            camera = Camera.main;
        }
        transform.rotation = Quaternion.LookRotation(camera.transform.TransformVector(Vector3.forward), camera.transform.TransformVector(Vector3.up));
    }
}
