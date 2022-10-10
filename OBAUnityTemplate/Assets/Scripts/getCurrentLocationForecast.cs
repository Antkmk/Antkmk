// -------------------------------------------------------------------------------
//
// getCurrentLocationForecast.cs
// Description:   Get the weather forecast at the device's current location.
//                Called from the "Current Location" button on the "weatherScene"
//                This routine calls:
//                (1) GetGPSLocation
//                (2) GetWeatherFile
//                (3) WeatherParser
//                (4) DisplayWeather
// Created:       08  / 22 / 22 © One Bad Ant
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
using TMPro;

public class getCurrentLocationForecast : MonoBehaviour
{
    // GPS data to display
    public TextMeshProUGUI currentGPSResult;
    public TextMeshProUGUI currentGPSResultLatitude;
    public TextMeshProUGUI currentGPSResultLongitude;


    // -------------------------------------------------------------------------------
    // On "Current Location" button click
    // -------------------------------------------------------------------------------
    public void getCurrentGPS()
    {
        getGPSLocation gpsLocation = FindObjectOfType<getGPSLocation>();

        if (Input.location.isEnabledByUser)
        {
            gpsLocation.StartLocation();
            StartCoroutine(WaitForGPSReturn());
        }
        else
        {
            currentGPSResult.SetText("User Disabled");
            currentGPSResultLatitude.SetText("0");
            currentGPSResultLongitude.SetText("0");
        }
    }

    // -------------------------------------------------------------------------------
    // Wait for a return from the "getGPSLocation" service
    // -------------------------------------------------------------------------------
    public IEnumerator WaitForGPSReturn()
    {
        getGPSLocation gpsLocation = FindObjectOfType<getGPSLocation>();

        // wait for location services to respond
        int maxWait = 20;
        while (gpsLocation.getGPSResultLatitude == "0" && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Once we have a result update the UI and stop the location service
        // -------------------------------------------------------------------------------
        currentGPSResult.SetText(gpsLocation.getGPSResult);
        currentGPSResultLatitude.SetText(gpsLocation.getGPSResultLatitude);
        currentGPSResultLongitude.SetText(gpsLocation.getGPSResultLongitude);

        gpsLocation.StopLocation();

        // -------------------------------------------------------------------------------
        // If we have a successful current GPS location result then get the weather file for the
        // start date and number of days (these could be passed here from another routine or
        // from the UI, but start date is simply being set to today and number of days to 3.
        // -------------------------------------------------------------------------------
        if (currentGPSResult.text == "Success")
        {
            getWeather getXMLWeatherFile = FindObjectOfType<getWeather>();

            getXMLWeatherFile.forecastStartDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            getXMLWeatherFile.forecastDays = 3;
            getXMLWeatherFile.forecastLocations = currentGPSResultLatitude.text + "," + currentGPSResultLongitude.text;
            getXMLWeatherFile.forecastFileName = "unityweather.xml";

            getXMLWeatherFile.StartFileDownload();

            // wait for file download to complete
            int maxDWait = 3;
            while (getXMLWeatherFile.fileGetStatus == "" && maxDWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxDWait--;
            }

            currentGPSResult.SetText(getXMLWeatherFile.fileGetStatus);

            // -------------------------------------------------------------------------------
            // If the generation and download of the XML weather forecast file was successful
            // then parse and store the weather forecast data in the database.
            // -------------------------------------------------------------------------------
            if (getXMLWeatherFile.fileGetStatus == "Success")
            {
                weatherParser parseXMLWeatherFile = FindObjectOfType<weatherParser>();

                parseXMLWeatherFile.xmlFileName = "unityweather.xml";

                parseXMLWeatherFile.StartParser();

                currentGPSResult.SetText(parseXMLWeatherFile.parserStatus);

                // -------------------------------------------------------------------------------
                // Finally display the weather forecast
                // -------------------------------------------------------------------------------
                displayWeather currentWeatherForecast = FindObjectOfType<displayWeather>();

                currentWeatherForecast.displayWeatherData();
            }

        }

    } // IEnumerator
}