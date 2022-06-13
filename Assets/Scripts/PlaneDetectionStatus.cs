using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneDetectionStatus : MonoBehaviour
{
    Text m_Status;
    ARPlaneManager m_PlaneManager;
    ARPointCloudManager m_PointManager;
    // Start is called before the first frame update
    void Start()
    {
        m_Status = GameObject.Find("Canvas/PlaneDetectionStatusText").GetComponent<Text>();
        m_PlaneManager = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>();
        m_PointManager = GameObject.Find("AR Session Origin").GetComponent<ARPointCloudManager>();
    }

    // Update is called once per frame
    void Update()
    {
        string planeStatus = "Plane Detection: " + m_PlaneManager.enabled + ", " + m_PlaneManager.trackables.count;
        string pointStatus = "Point Detection: " + m_PointManager.enabled + ", " + m_PointManager.trackables.count;
        m_Status.text = planeStatus + "\n" + pointStatus;
    }
}
