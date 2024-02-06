using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class ScreenInputManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text prompt;

    // function to execute in VR space
    private Action<Pose> hitAction;
    // function to execute in screen space
    private Action<EnhancedTouch.Finger> fingerAction;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits = new();

    private int fingerDownCount;


    public void SetActionOnFingerDown(Action<Pose> action)
    {
        hitAction = new(action);
        fingerAction = null;
    }

    public void SetActionOnFingerDown(Action<EnhancedTouch.Finger> action)
    {
        fingerAction = new(action);
        hitAction = null;
    }

    public void ResetActionOnFingerDown()
    {
        hitAction = null;
        fingerAction = null;
    }


    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        fingerDownCount = 0;
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += RegisterFingerDown;
        EnhancedTouch.Touch.onFingerDown += OnFingerDown;

    }

    private void OnDisable()
    {
        EnhancedTouch.Touch.onFingerDown -= RegisterFingerDown;
        EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();


    }

    private void OnFingerDown(EnhancedTouch.Finger finger)
    {
        // Only recognize one finger
        if (finger.index == 0)
        {
            // work in AR space
            if (hitAction != null)
            {
                if (raycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    hitAction(hits[0].pose);
                }
            }
            // work in screen space
            else if (fingerAction != null)
            {
                fingerAction(finger);
            }
        }
    }

    private void RegisterFingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index == 0)
        {
            // first tap spawns landmark
            if (fingerDownCount == 0)
            {
                //SetActionOnFingerDown(GetComponent<LandmarkTracker>().Spawn);
            }
            // second tap spawns creature (CAPYBARA)
            else if (fingerDownCount == 1)
            {
               //SetActionOnFingerDown(GetComponent<CreatureTracker>().Spawn);
            }
            // any subsequent taps throw balls
            else
            {
                //SetActionOnFingerDown(Camera.main.GetComponent<Thrower>().Throw);
            }
            ++fingerDownCount;
        }
    }
}
