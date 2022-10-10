// -------------------------------------------------------------------------------
//
// getGPSLocation.cs
// Description:   Get the current GPS location of the device.
// Created:       07 / 27 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;


public class getGPSLocation : MonoBehaviour
{
    // Publically accessible data
    public string getGPSResult;
    public string getGPSResultLatitude;
    public string getGPSResultLongitude;
    public string getGPSResultAltitude;
    public string getGPSResultTimestamp;
    public string getGPSResultHorizontalAccuracy;

    private bool startTracking = false;


    // Start the location service
    public void StartLocation()
    {
        if (Input.location.isEnabledByUser)
        {
            StartCoroutine(GetCurrentLocation());
        }
    }

    // Stop the location service
    public void StopLocation()
    {
        startTracking = false;
        Input.location.Stop();
        getGPSResult = "Location Services are stopped.";
        Debug.Log(getGPSResult);
    }

    // See if the location services can be started
    // -------------------------------------------------------------------------------
    public IEnumerator GetCurrentLocation()
    {
        getGPSResult = "Location Services started.";
        getGPSResultLatitude = "0";
        getGPSResultLongitude = "0";

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }

        if (Input.location.isEnabledByUser)
        {
            Input.location.Start(10.0f,10.0f);
            int maxWait = 20;

            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {

                Debug.Log("GPS Loop: " + Input.location.status + " = " + LocationServiceStatus.Initializing + " " + maxWait.ToString());
                getGPSResult = Input.location.status + " GPS: " + maxWait.ToString();
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // If the service didn't initialize in 20 seconds this cancels location service use.
            if (maxWait < 1)
            {
                getGPSResult = "GPS Timed out";
                Input.location.Stop();
                yield break;
            }

            // If the connection failed this cancels location service use.
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                getGPSResult = "Unable to determine device location";
                Input.location.Stop();
                yield break;
            }
            else
            {
                startTracking = true;
                yield break;
            }
        }

        // User has not enabled location services
        else
        {
            getGPSResult = "Location Services are not enabled by user.";
        }
    } // IEnumerator


    // Continuously read the device's GPS location
    // -------------------------------------------------------------------------------
    void Update()
    {
        if (startTracking)
        {
            getGPSResult = "Success";
            getGPSResultLatitude = Input.location.lastData.latitude.ToString();
            getGPSResultLongitude = Input.location.lastData.longitude.ToString();
            getGPSResultAltitude = Input.location.lastData.altitude.ToString();
            getGPSResultTimestamp = Input.location.lastData.timestamp.ToString();
            getGPSResultHorizontalAccuracy = Input.location.lastData.horizontalAccuracy.ToString();
        }
    }

}