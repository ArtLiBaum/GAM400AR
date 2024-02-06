using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RemoveObjectOnTouch: MonoBehaviour
{
    [SerializeField] private Camera raycastCamera;

    void Start()
    {

    }

    void Update()
    {
        // Check if the user touches the screen
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Raycast from the touch position
            Ray ray = raycastCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is a tracked AR object
                GameObject trackedObject = hit.collider.gameObject;
                if (trackedObject != null)
                {
                    // Destroy the object
                    Destroy(trackedObject);
                }
            }

        }
    }
}