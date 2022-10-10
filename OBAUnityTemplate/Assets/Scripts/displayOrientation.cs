// -------------------------------------------------------------------------------
//
// displayOrientation.cs
// Description:   Display the current orientation from the device's magnetometer
//                found by the Get Orientation prefab.
// Created:       08 / 17 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class displayOrientation : MonoBehaviour
{
    public TextMeshProUGUI compRawVector;
    public TextMeshProUGUI compMagHeading;
    public TextMeshProUGUI compTrueHeading;
    public TextMeshProUGUI compDegrees;
    public TextMeshProUGUI compStatusMsg;

    void Start()
    {
        compStatusMsg.SetText("Press 'Start' and 'Stop' to control the Compass Service.");
    }

    // Continuously query and update the display with the readings from the
    // "Get Orientation" "getMagnetometer" script
    // -------------------------------------------------------------------------------
    void Update()
    {
        getMagnetometer magnetometer = FindObjectOfType<getMagnetometer>();

        if (magnetometer.compassStatus == "Started")
        {
            compStatusMsg.SetText("Compass Service Running.");
            compRawVector.SetText(magnetometer.magRawVector.ToString());
            compMagHeading.SetText(magnetometer.magHeading.ToString());
            compTrueHeading.SetText(magnetometer.magTrueHeading.ToString());
            compDegrees.SetText(magnetometer.compassDegrees);
        }
    }

    // 'Start' button pressed to begin the magnetometer service
    // -------------------------------------------------------------------------------
    public void startCompassRequest()
    {
        getMagnetometer magnetometer = FindObjectOfType<getMagnetometer>();
        compStatusMsg.SetText("Start Compass service requested.");
        magnetometer.StartReadings();
    }

    // 'Stop' button pressed to end the magnetometer service
    // -------------------------------------------------------------------------------
    public void stopCompassRequest()
    {
        getMagnetometer magnetometer = FindObjectOfType<getMagnetometer>();
        magnetometer.StopReadings();
        compStatusMsg.SetText("Compass service stopped.");
    }
}