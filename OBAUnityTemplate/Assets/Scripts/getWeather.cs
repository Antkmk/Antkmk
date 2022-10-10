// -------------------------------------------------------------------------------
//
// getWeather.cs
// Description:   Get the weather given set of locations and dates.
//                The weather is from graphical.weather.gov. This routine generates
//                and downloads an XML file from a GPS Lat and Lon list, start date and
//                number of days.
// Input:         (1) Comma-delimited list of latitude and longitude weather locations
//                (2) Start date of the weather forecast
//                (3) Number of days of forecast to retrieve
//                (4) Name to give the downloaded file
// Output:        (1) File status
//                (2) Path to the downloaded file
//                (3) XML weather forecast file
// Created:       08 / 01 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class getWeather : MonoBehaviour
{
    // Forecast (input) parameters
    public string forecastLocations;
    public string forecastStartDate;
    public int forecastDays;
    public string forecastFileName;

    // Result (ouput) parameters
    public string fileGetStatus;
    public string filePath;

    // Format the URL to generate and download the XML weather forecast file
    // -------------------------------------------------------------------------------
    public void StartFileDownload()
    {
        fileGetStatus = "";

        string fileURL = "https://graphical.weather.gov/xml/sample_products/browser_interface/ndfdBrowserClientByDay.php?whichClient=NDFDgenByDayLatLonList&listLatLon=" + forecastLocations + "&format=24+hourly&startDate=" + forecastStartDate + "&numDays=" + forecastDays + "&Unit=e";

        Debug.Log("File URL: " + fileURL);

        StartCoroutine(GetFile(fileURL));
    }

    // Download the Weather File
    // -------------------------------------------------------------------------------
    IEnumerator GetFile(string fileURL)
    {
        UnityWebRequest weatherRequest = new UnityWebRequest(fileURL, UnityWebRequest.kHttpVerbGET);
        string path = Path.Combine(Application.persistentDataPath, forecastFileName);

        weatherRequest.downloadHandler = new DownloadHandlerFile(path);

        yield return weatherRequest.SendWebRequest();
        if (weatherRequest.result != UnityWebRequest.Result.Success)
        {
            fileGetStatus = "Error: " + weatherRequest.error;
            Debug.Log(fileGetStatus);
        }
        else
        {
            fileGetStatus = "Success";
            filePath = path;
            Debug.Log(fileGetStatus + " " + filePath);
        }
    }
}