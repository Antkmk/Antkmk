// -------------------------------------------------------------------------------
//
// getMultiLocationForecast.cs
// Description:   Get the weather forecast given a list of locations (latitudes
//                and longitudes).
//                Called from the "Mutiple Locations" button on the "weatherScene"
//                This routine calls:
//                (1) GetWeatherFile
//                (2) WeatherParser
//                (3) DisplayWeather
// Created:       09  / 07 / 22 © One Bad Ant
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

public class getMultiLocationForecast : MonoBehaviour
{
    public void initMultiForecast()
    {
        StartCoroutine(getMultiWeatherFile());
    }

    // -------------------------------------------------------------------------------
    // On "Multiple Locations" button click
    // -------------------------------------------------------------------------------
    public IEnumerator getMultiWeatherFile()
    {
        // -------------------------------------------------------------------------------
        // Get the weather file for the start date and number of days (these could be passed
        // here from another routine or from the UI, but start date is simply being set to
        // today and number of days to 3.
        // Also the list of lat and longs is simply given here.
        // -------------------------------------------------------------------------------
        getWeather getXMLWeatherFile = FindObjectOfType<getWeather>();

        getXMLWeatherFile.forecastStartDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        getXMLWeatherFile.forecastDays = 3;
        getXMLWeatherFile.forecastLocations = "40.77,-73.98+42.41,-76.63+42.93,-75.86+43.71,-74.97";
        getXMLWeatherFile.forecastFileName = "unityweather.xml";

        getXMLWeatherFile.StartFileDownload();

        // wait for file download to complete
        int maxDWait = 3;
        while (getXMLWeatherFile.fileGetStatus == "" && maxDWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxDWait--;
        }

        Debug.Log("file? " + getXMLWeatherFile.fileGetStatus);

        // -------------------------------------------------------------------------------
        // If the generation and download of the XML weather forecast file was successful
        // then parse and store the weather forecast data in the database.
        // -------------------------------------------------------------------------------
        if (getXMLWeatherFile.fileGetStatus == "Success")
        {
            weatherParser parseXMLWeatherFile = FindObjectOfType<weatherParser>();

            parseXMLWeatherFile.xmlFileName = "unityweather.xml";

            parseXMLWeatherFile.StartParser();

            // -------------------------------------------------------------------------------
            // Finally display the weather forecast
            // -------------------------------------------------------------------------------
            displayWeather currentWeatherForecast = FindObjectOfType<displayWeather>();

            currentWeatherForecast.displayWeatherData();
        }

    } // getMultiWeatherFile
}