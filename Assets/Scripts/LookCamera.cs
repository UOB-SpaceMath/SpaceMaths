using UnityEngine;

public class LookCamera : MonoBehaviour
{
    [SerializeField]
    private CameraManager _cm;

    void LateUpdate()
    {
        Camera camera = _cm.GetCamera();
        transform.rotation = Quaternion.LookRotation(camera.transform.TransformVector(Vector3.forward), camera.transform.TransformVector(Vector3.up));
    }
}
