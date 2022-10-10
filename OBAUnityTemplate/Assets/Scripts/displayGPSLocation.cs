// -------------------------------------------------------------------------------
//
// displayGPSLocation.cs
// Description:   Display the current GPS location of the device from the
//                "GetGPSLocation" prefab
//                The data, gathered by the prefab, and the display, handled by
//                this routine, are separate so data gathering and display can be
//                used separately and in multiple places if needed.
// Created:       08 / 16 / 22 © One Bad Ant
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


public class displayGPSLocation : MonoBehaviour
{
    public TextMeshProUGUI latValue;
    public TextMeshProUGUI lonValue;
    public TextMeshProUGUI altValue;
    public TextMeshProUGUI accValue;
    public TextMeshProUGUI statusMsg;

    void Start()
    {
        statusMsg.SetText("Press 'Start' and 'Stop' to control the GPS Service.");
    }

    // Constantly queue and display the GPS data as long as the the service is
    // running: result = success
    // -------------------------------------------------------------------------------
    void Update()
    {
        getGPSLocation gpsLocation = FindObjectOfType<getGPSLocation>();

        if (Input.location.isEnabledByUser && gpsLocation.getGPSResult == "Success")
        {
            statusMsg.SetText(gpsLocation.getGPSResult);
            latValue.SetText(gpsLocation.getGPSResultLatitude);
            lonValue.SetText(gpsLocation.getGPSResultLongitude);
            altValue.SetText(gpsLocation.getGPSResultAltitude);
            accValue.SetText(gpsLocation.getGPSResultHorizontalAccuracy);
        }
    }

    // 'Start' button pressed to begin the location service
    // -------------------------------------------------------------------------------
    public void startGPSRequest()
    {
        if (!(Input.location.isEnabledByUser))
        {
            statusMsg.SetText("Location Services are not enabled by user.");
        }
        else
        {
            getGPSLocation gpsLocation = FindObjectOfType<getGPSLocation>();
            statusMsg.SetText("Start GPS service requested.");
            gpsLocation.StartLocation();
        }
    }

    // 'Stop' button pressed to end the location service
    // -------------------------------------------------------------------------------
    public void stopGPSRequest()
    {
        getGPSLocation gpsLocation = FindObjectOfType<getGPSLocation>();
        statusMsg.SetText("GPS service stopped.");
        gpsLocation.StopLocation();
    }
}