using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class AnchorCreator : MonoBehaviour
{
    [SerializeField]
    GameObject m_GameScene;

    public GameObject gameScene
    {
        get => m_GameScene;
        set => m_GameScene = value;
    }

    public void RemoveAnchor()
    {
        if (m_CurrentAnchors)
        {
            Destroy(m_CurrentAnchors.gameObject);
            m_CurrentAnchors = null;
            TogglePlaneDetection();
        }
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        var oldPrefab = m_AnchorManager.anchorPrefab;
        m_AnchorManager.anchorPrefab = gameScene;
        var anchor = m_AnchorManager.AttachAnchor((ARPlane)hit.trackable, hit.pose);
        m_AnchorManager.anchorPrefab = oldPrefab;
        TogglePlaneDetection();
        return anchor;
    }

    void Update()
    {
        // no current anchor
        if (m_CurrentAnchors is null)
        {
            // get touching input
            if (Input.touchCount == 0)
                return;

            var touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;
            // Perform the raycast
            if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one will be the closest hit.
                var hit = s_Hits[0];
                var anchor = CreateAnchor(hit);
                if (anchor)
                {
                    // Remember the anchor so we can remove it later.
                    m_CurrentAnchors = anchor;
                }
                else
                {
                    // todo add android toast message.
                }
            }
        }

    }

    public void TogglePlaneDetection()
    {
        if (m_PlaneManager.enabled is true)
        {
            m_PlaneManager.SetTrackablesActive(false);
        }
        else
        {
            m_PlaneManager.SetTrackablesActive(true);
        }
        m_PlaneManager.enabled = !m_PlaneManager.enabled;
    }

    static readonly List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARAnchor m_CurrentAnchors;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
