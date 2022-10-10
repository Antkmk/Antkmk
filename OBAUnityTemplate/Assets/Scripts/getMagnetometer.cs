// -------------------------------------------------------------------------------
//
// getMagnetometer.cs
// Description:   Gets the device's current magnetometer readings. Includes methods
//                to start and stop magnetometer services. The data gathered by this
//                prefab is handled separately from any display of the data so the
//                prefab can be used in multiple scenes and multiple purposes in the
//                app as needed.
// Created:       07 / 28 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class getMagnetometer : MonoBehaviour
{

    public string magRawVector;
    public float magHeading;
    public float magTrueHeading;
    public string compassDegrees;
    public string compassStatus;

    private bool startTracking = false;

    // Start the magnetometer
    public void StartReadings()
    {
        compassStatus = "Start Requested";
        Input.compass.enabled = true;
        Input.location.Start();
        StartCoroutine(InitializeCompass());
    }

    // Stop the magnetometer
    public void StopReadings()
    {
        startTracking = false;
        compassStatus = "Stopped";
        Input.compass.enabled = false;
        Input.location.Stop();
    }

    IEnumerator InitializeCompass()
    {
        yield return new WaitForSeconds(1f);
        startTracking |= Input.compass.enabled;
    }

    // Continuously read the device's magnetometer
    // -------------------------------------------------------------------------------
    void Update()
    {
        if (startTracking)
        {
            compassStatus = "Started";
            magRawVector = Input.compass.rawVector.ToString(); // raw vector
            magHeading = (float)Math.Round(Input.compass.magneticHeading); // magnetometer heading
            magTrueHeading = (float)Math.Round(Input.compass.trueHeading); // magnetometer true heading
            compassDegrees = (int)Input.compass.trueHeading + "° " + DegreesToCardinalDetailed(Input.compass.trueHeading); // compass bearing
        }
    }

    // Convert magnetometer's true heading to compass bearing
    // -------------------------------------------------------------------------------
    private static string DegreesToCardinalDetailed(double degrees)
    {
        string[] caridnals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
        return caridnals[(int)Math.Round(((double)degrees * 10 % 3600) / 225)];
    }
}