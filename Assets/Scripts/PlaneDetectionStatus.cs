using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneDetectionStatus : MonoBehaviour
{
    Text m_Status;
    ARPlaneManager m_PlaneManager;
    // Start is called before the first frame update
    void Start()
    {
        m_Status = GameObject.Find("Canvas/PlaneDetectionStatusText").GetComponent<Text>();
        m_PlaneManager = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_PlaneManager.enabled is true)
        {
            m_Status.text = "Plane Manager: enabled";
        }
        else
        {
            m_Status.text = "Plane Manager: disabled";
        }
    }
}
