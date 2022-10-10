// -------------------------------------------------------------------------------
//
// displayWeather.cs
// Description:   Display the weather records stored in a database.
// Created:       08 / 11 / 22 © One Bad Ant
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
using System.Data;
using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using Mono.Data.Sqlite;
using TMPro;


public class displayWeather : MonoBehaviour
{
    // Define data structures and variables
    // ---------------------------------------------------------------------------------------

    // Database variables
    string sqlQuery;
    string sqlQuery2;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader dbreader;
    IDbCommand dbcmd2;
    IDataReader dbreader2;
 
    public TextMeshProUGUI lastUpdateDate;
    private TextMeshProUGUI pointKey;
    private TextMeshProUGUI dailyForecast;


    // Start is called before the first frame update
    public void displayWeatherData()
    {
        GameObject mainCanvas = GameObject.Find("weatherCanvas");

        // Open a connection to the database
        using (dbconn = new SqliteConnection(SceneParametersHandler.DATABASE_CONN))
        {
            dbconn.Open();

            // Read the weather from the database and display the forecasts
            // ---------------------------------------------------------------------------------------
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT * FROM weather_point ORDER BY wp_point_key ASC";
            dbcmd.CommandText = sqlQuery;
            dbreader = dbcmd.ExecuteReader();
            int pointCount = 0;
            while (dbreader.Read())
            {
                pointCount++;
                // Display the date the forecast was last updated
                // ---------------------------------------------------------------------------------------
                if (pointCount == 1)
                {
                    DateTime uDate = Convert.ToDateTime(dbreader["wp_last_updated"]);
                    lastUpdateDate.SetText(uDate.ToString("MM/dd/yyyy"));
                }

                // Display the weather points: locations
                // ---------------------------------------------------------------------------------------
                GameObject weatherPoint = new GameObject("weatherPoint", typeof(RectTransform));
                weatherPoint.transform.SetParent(mainCanvas.transform);

                TextMeshProUGUI pointKeyLatLon = weatherPoint.AddComponent<TextMeshProUGUI>();
                pointKeyLatLon.fontSize = 48;
                pointKeyLatLon.text = dbreader["wp_point_key"] + "\n" + dbreader["wp_latitude"] + "\n" + dbreader["wp_longitude"];
                pointKeyLatLon.alignment = TextAlignmentOptions.Left;

                float YPos = -200 + (-360 * pointCount); // Y position of text object

                weatherPoint.GetComponent<RectTransform>().localPosition = new Vector3(-450f, YPos, 0f); // set position
                weatherPoint.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1.0f);
                weatherPoint.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1.0f);
                weatherPoint.GetComponent<RectTransform>().sizeDelta = new Vector2(400.0f, 100.0f); // set the object's size
                weatherPoint.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

                // Display the point's daily weather forecasts
                // ---------------------------------------------------------------------------------------
                dbcmd2 = dbconn.CreateCommand();
                sqlQuery2 = "SELECT * FROM daily_weather WHERE dw_point_key = '" + dbreader["wp_point_key"] + "' ORDER BY dw_datetime ASC";
                dbcmd2.CommandText = sqlQuery2;
                dbreader2 = dbcmd2.ExecuteReader();

                int dailyCount = 0;
                while (dbreader2.Read())
                {
                    dailyCount++;
                    GameObject dailyWeather = new GameObject("dailyWeather", typeof(RectTransform));
                    dailyWeather.transform.SetParent(mainCanvas.transform);

                    TextMeshProUGUI dailyForecast = dailyWeather.AddComponent<TextMeshProUGUI>();
                    dailyForecast.fontSize = 48;
                    DateTime dfDate = Convert.ToDateTime(dbreader2["dw_datetime"]);
                    dailyForecast.text = dfDate.ToString("MM/dd/yyyy") + "\nMax:" + dbreader2["dw_max_temp"] + "\nMin:" + dbreader2["dw_min_temp"] + "\n" + dbreader2["dw_conditions"];
                    dailyForecast.alignment = TextAlignmentOptions.Left;

                    float dwYPos = -200 + (-360 * pointCount);
                    float dwXPos = -640 + (400 * dailyCount);

                    dailyWeather.GetComponent<RectTransform>().localPosition = new Vector3(dwXPos, dwYPos, 0f); // set position
                    dailyWeather.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
                    dailyWeather.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
                    dailyWeather.GetComponent<RectTransform>().sizeDelta = new Vector2(400.0f, 100.0f);
                    dailyWeather.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

                } // while dbreader2

            } // while dbreader

            dbconn.Close();

        } // using dbconn

    } // Start
}