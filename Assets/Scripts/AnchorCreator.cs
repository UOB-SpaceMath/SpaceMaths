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
    [SerializeField]
    GameObject _gameBoard;

    [SerializeField]
    GameObject _gameManager;

    [SerializeField]
    Button _clearButton;

    static readonly List<ARRaycastHit> _Hits = new List<ARRaycastHit>();

    ARRaycastManager _raycastManager;

    ARAnchorManager _anchorManager;

    ARPlaneManager _planeManager;

    ARPointCloudManager _pointCloudManager;

    ARAnchor _currentAnchors;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _anchorManager = GetComponent<ARAnchorManager>();
        _planeManager = GetComponent<ARPlaneManager>();
        _pointCloudManager = GetComponent<ARPointCloudManager>();
        // get and hide game board if the game is not run in Unity Editor.
        if (Application.platform != RuntimePlatform.OSXEditor &&
            Application.platform != RuntimePlatform.LinuxEditor &&
            Application.platform != RuntimePlatform.WindowsEditor)
        {
            _gameManager.SetActive(false);
            _gameBoard.SetActive(false);
        }

        // get clear button
        _clearButton.interactable = false;
    }

    void Update()
    {
        // no current anchor
        if (_currentAnchors is null)
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
            if (_raycastManager.Raycast(touch.position, _Hits, trackableTypes))
            {
                // Raycast hits are sorted by distance, so the first one will be the closest hit.
                var hit = _Hits[0];
                var anchor = ShowGameBoardOnPlane(hit);
                if (anchor)
                {
                    // Remember the anchor so we can remove it later.
                    _currentAnchors = anchor;
                }
                else
                {
                    Debug.Log("fail to set anchor.");
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
            anchor = _anchorManager.AttachAnchor(plane, hit.pose);
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
        _gameManager.SetActive(true);
        _gameBoard.SetActive(true);
        _gameBoard.transform.SetPositionAndRotation(anchor.transform.position, anchor.transform.rotation);
        // turn off plane detection
        ToggleDetection();
        // set button active
        _clearButton.interactable = true;

        return anchor;
    }

    public void RemoveAnchor()
    {
        if (_currentAnchors)
        {
            Destroy(_currentAnchors.gameObject);
            _currentAnchors = null;
            _gameManager.SetActive(false);
            _gameBoard.SetActive(false);
            ToggleDetection();
        }
        _clearButton.interactable = false;
    }

    public void ToggleDetection()
    {
        _planeManager.enabled = !_planeManager.enabled;
        _planeManager.SetTrackablesActive(_planeManager.enabled);
        _pointCloudManager.enabled = !_pointCloudManager.enabled;
        _pointCloudManager.SetTrackablesActive(_pointCloudManager.enabled);
    }


}
