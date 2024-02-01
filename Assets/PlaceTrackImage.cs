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
    public int nonCount = 0;
    public TMP_Text trackingNumber;
    public TMP_Text limitedNumber;
    public TMP_Text nonNumber;

    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();
    void Update()
    {
        trackingNumber.text = trackingCount.ToString();
        limitedNumber.text = limitedCount.ToString();
        nonNumber.text = nonCount.ToString();
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

    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        
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
                   
                }
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                trackingCount += 1;
            }
            else if(trackedImage.trackingState == TrackingState.Limited)
            {
                limitedCount += 1;
            }
            else if(trackedImage.trackingState == TrackingState.None)
            {
                nonCount += 1;
            }
            //_instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
            _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
        }

    }
}
