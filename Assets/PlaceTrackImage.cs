using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackImage : MonoBehaviour
{
    private ARTrackedImageManager _trackImageManager;

    public GameObject[] ArPrefabs;
    public int trackingCount = 0;
    public int limitedCount = 0;
    public int spawnCount = 0;
    public TMP_Text trackingNumber;
    public TMP_Text limitedNumber;
    public TMP_Text spawnNumber;

    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();
    void Update()
    {
        trackingNumber.text = trackingCount.ToString();
        limitedNumber.text = limitedCount.ToString();
        spawnNumber.text = spawnCount.ToString();
    }
    void Awake()
    {
        _trackImageManager = GetComponent<ARTrackedImageManager>();    
    }
    void OnEnable()
    {
        _trackImageManager.trackedImagesChanged += OnTrackedImageChanged;

    }
    void OnDisable()
    {
        _trackImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }

    // This function got called every frame after the first image is trackd in ARCore
    // However, the behavior is different on ARKit, this function only being called when image is tracking
    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Called once when a specific image is called
        foreach (var trackedImage in eventArgs.added)
        {
            var imageName = trackedImage.referenceImage.name;

            foreach(var curPrefab in ArPrefabs)
            {
                if(string.Compare(curPrefab.name, imageName, StringComparison.OrdinalIgnoreCase) == 0
                    && !_instantiatedPrefabs.ContainsKey(imageName))
                {
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    _instantiatedPrefabs[imageName] = newPrefab;
                    spawnCount += 1;
                }
            }
        }
        // Called every frame if it is tracking any image 
        foreach (var trackedImage in eventArgs.updated)
        {
            // While image is tracking 
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                trackingCount += 1;
            }
            // On ARCore, trackingState is limited if an image is not fully tracked 
            // On ARKit, trackingState is only being set to limited once because as long as it is not fulling tracked,
            // because "OnTrackedImageChanged" is not being called anymore
            else if (trackedImage.trackingState == TrackingState.Limited)
            {
                limitedCount += 1;
            }
            // Tracking state rarely being set to non
            else if(trackedImage.trackingState == TrackingState.None)
            {
                
            }
            //_instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }
        // Nothing will be set to removed
        foreach (var trackedImage in eventArgs.removed)
        {
            Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
            _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
        }

    }
}
