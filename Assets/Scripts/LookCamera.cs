using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    private GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.Find("AR Session Origin/AR Camera");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Camera.transform.TransformVector(Vector3.forward), Camera.transform.TransformVector(Vector3.up));
    }
}
