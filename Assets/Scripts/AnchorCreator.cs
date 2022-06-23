using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARPointCloudManager))]
public class AnchorCreator : MonoBehaviour
{

    static readonly List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;

    ARPointCloudManager m_PointCloudManager;

    ARAnchor m_CurrentAnchors;

    GameObject m_GameBoard;

    Button m_ClearButton;


    void Start()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_PointCloudManager = GetComponent<ARPointCloudManager>();
        // get and hide game board if the game is not run in Unity Editor.
        if (Application.platform != RuntimePlatform.OSXEditor &&
            Application.platform != RuntimePlatform.LinuxEditor &&
            Application.platform != RuntimePlatform.WindowsEditor)
        {
            m_GameBoard = GameObject.Find("GameBoard");
            m_GameBoard.SetActive(false);
        }


        // get clear buttom
        m_ClearButton = GameObject.Find("AR Canvas/Button").GetComponent<Button>();
        m_ClearButton.interactable = false;

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

            // Raycast against planes and feature points
            const TrackableType trackableTypes =
                TrackableType.FeaturePoint |
                TrackableType.PlaneWithinPolygon;

            // Perform the raycast
            if (m_RaycastManager.Raycast(touch.position, s_Hits, trackableTypes))
            {
                // Raycast hits are sorted by distance, so the first one will be the closest hit.
                var hit = s_Hits[0];
                var anchor = ShowGameBoardOnPlane(hit);
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

    ARAnchor ShowGameBoardOnPlane(in ARRaycastHit hit)
    {
        ARAnchor anchor;
        // try to create anchor on plane
        if (hit.trackable is ARPlane plane)
        {
            anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
        }
        else
        {
            // create gameObject on position
            var gameObject = Instantiate(new GameObject(), hit.pose.position, hit.pose.rotation);
            // Make sure the new GameObject has an ARAnchor component
            anchor = gameObject.GetComponent<ARAnchor>();
            if (anchor is null)
            {
                anchor = gameObject.AddComponent<ARAnchor>();
            }
        }

        // attach game board on anchor;
        m_GameBoard.SetActive(true);
        m_GameBoard.transform.SetPositionAndRotation(anchor.transform.position, anchor.transform.rotation);
        // turn off plane detection
        ToggleDetection();
        // set button 
        m_ClearButton.interactable = true;

        return anchor;
    }

    public void RemoveAnchor()
    {
        if (m_CurrentAnchors)
        {
            Destroy(m_CurrentAnchors.gameObject);
            m_CurrentAnchors = null;
            m_GameBoard.SetActive(false);
            ToggleDetection();
        }
        m_ClearButton.interactable = false;
    }

    public void ToggleDetection()
    {
        m_PlaneManager.enabled = !m_PlaneManager.enabled;
        m_PlaneManager.SetTrackablesActive(m_PlaneManager.enabled);
        m_PointCloudManager.enabled = !m_PointCloudManager.enabled;
        m_PointCloudManager.SetTrackablesActive(m_PointCloudManager.enabled);
    }


}
