// -------------------------------------------------------------------------------
//
// weatherParser.cs
// Description:   Parse the XML weather file from graphical.weather.gov.
//                This file would have already been downloaded and stored locally
//                by the "getWeather" script.
//                The weather file can have multiple locations (GPS points) and
//                multiple days per point.
//                The weather will be stored in a database so the last recorded
//                forecast can be retrieved even without an internet connection.
// Input:         (1) The file name of the XML weather forecast file
// Output:        (1) Parsing status
// Created:       08 / 01 / 22 © One Bad Ant
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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using Mono.Data.Sqlite;


public class weatherParser : MonoBehaviour
{
    // Parser (input) parameters
    public string xmlFileName;

    // Result (ouput) parameters
    public string parserStatus;


    // Define data structures and variables
    // ---------------------------------------------------------------------------------------

    // Database variables
    string sqlQuery;
    string sqlUpdateQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDbCommand dbUpdatecmd;
    IDataReader dbreader;

    List<string> filePointKeys = new List<string>(); // List of points in the weather file ("GPS point")

    string today = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");


    // ---------------------------------------------------------------------------------------
    // Start parser
    // ---------------------------------------------------------------------------------------
    public void StartParser()
    {
        // ---------------------------------------------------------------------------------------
        // Empty the weather tables each time before we get new weather data
        // ---------------------------------------------------------------------------------------
        using (dbconn = new SqliteConnection(SceneParametersHandler.DATABASE_CONN))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = "DELETE FROM weather_point;";
            dbcmd.ExecuteScalar();
            dbcmd.CommandText = "DELETE FROM daily_weather;";
            dbcmd.ExecuteScalar();
            dbconn.Close();
        }

        // ---------------------------------------------------------------------------------------
        // Load the weather XML file
        // ---------------------------------------------------------------------------------------
        string filePath = Path.Combine(Application.persistentDataPath, xmlFileName);
        XElement xmlFile = XElement.Load(filePath);
        IEnumerable<XElement> weatherElements = xmlFile.Elements();


        // Open a connection to the database to store parsed data from the XML file
        using (dbconn = new SqliteConnection(SceneParametersHandler.DATABASE_CONN))
        {
            dbconn.Open();

            // ---------------------------------------------------------------------------------------
            // Get and store the weather LOCATIONS : point, latitude, longitude
            // ---------------------------------------------------------------------------------------
            foreach (XElement latLonRec in weatherElements.Descendants("location"))
            {
                filePointKeys.Add(latLonRec.Element("location-key").Value); // hold on to a list of points (element keys)

                dbcmd = dbconn.CreateCommand();
                sqlQuery = "INSERT INTO [weather_point] ( [wp_point_key], [wp_latitude], [wp_longitude], [wp_last_updated] ) VALUES ";
                sqlQuery += "('" + latLonRec.Element("location-key").Value + "', ";
                sqlQuery += latLonRec.Element("point").Attribute("latitude").Value + ", ";
                sqlQuery += latLonRec.Element("point").Attribute("longitude").Value + ", ";
                sqlQuery += "'" + today + "')";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();
            }

            // ---------------------------------------------------------------------------------------
            // Get the DAYS included in this weather file
            // Create a daily weather record for each gps point
            // ---------------------------------------------------------------------------------------
            int oneDaySetOnly = 0; // only need the first set of days

            IEnumerable<XElement> tl24Hourlies = from tl24Eles in weatherElements.Descendants("time-layout")
                                             where tl24Eles.Attribute("summarization").Value == "24hourly"
                                             select tl24Eles;
            foreach (var this24Element in tl24Hourlies)
            {
                IEnumerable<XElement> days = this24Element.Descendants();

                foreach (XElement day in days)
                {
                    if (day.Name == "layout-key") {oneDaySetOnly++;}

                    if (day.Name == "start-valid-time" && oneDaySetOnly < 2)
                    {
                        for (int i = 0; i <= filePointKeys.Count - 1; i++)
                        {
                            dbcmd = dbconn.CreateCommand();
                            sqlQuery = "INSERT INTO [daily_weather] ( [dw_point_key], [dw_datetime] ) VALUES ";
                            sqlQuery += "('" + filePointKeys[i] + "', '" + day.Value + "')";
                            dbcmd.CommandText = sqlQuery;
                            dbcmd.ExecuteScalar();
                        }
                    }
                }
            }

            // ---------------------------------------------------------------------------------------
            // Loop through the parameter elements by weather location (point)
            // Get the max and min temps and forecast and store them in the daily weather records
            // created above.
            // ---------------------------------------------------------------------------------------
            for (int i = 0; i <= filePointKeys.Count - 1; i++)
            {
                IEnumerable<XElement> parameter = from pEle in weatherElements.Descendants("parameters")
                                                  where pEle.Attribute("applicable-location").Value == filePointKeys[i]
                                                  select pEle;
                foreach (XElement thisParameter in parameter)
                {
                    // Get MAXIMUM DAILY TEMPERATURES for each weather location (point)
                    // ---------------------------------------------------------------------------------------
                    List<string> pointMaxTemps = new List<string>(); // Create a temporary list of maximum temps for a point (location)

                    var maxtemps = from ele in thisParameter.Descendants("temperature")
                                   where ele.Element("name").Value == "Daily Maximum Temperature"
                                   select ele;
                    foreach (var maxtemp in maxtemps)
                    {
                        IEnumerable<XElement> maxTempElements = maxtemp.Descendants();
                        foreach (XElement maxDailyTemp in maxTempElements)
                        {
                            if (maxDailyTemp.Value != "Daily Maximum Temperature") { pointMaxTemps.Add(maxDailyTemp.Value); }
                        }
                    }

                    // Get MINIMUM DAILY TEMPERATURES for each weather location (point)
                    // ---------------------------------------------------------------------------------------
                    List<string> pointMinTemps = new List<string>(); // Create a temporary list of minimum temps for a point (location)

                    var mintemps = from ele in thisParameter.Descendants("temperature")
                                   where ele.Element("name").Value == "Daily Minimum Temperature"
                                   select ele;
                    foreach (var mintemp in mintemps)
                    {
                        IEnumerable<XElement> minTempElements = mintemp.Descendants();
                        foreach (XElement minDailyTemp in minTempElements)
                        {
                            if (minDailyTemp.Value != "Daily Minimum Temperature") { pointMinTemps.Add(minDailyTemp.Value); }
                        }
                    }

                    // Get WEATHER CONDITIONS for each weather location (point)
                    // ---------------------------------------------------------------------------------------
                    List<string> pointWeatherConditions = new List<string>(); // Create a temporary list of weather conditions for a point (location)

                    var conditions = from conditionElements in thisParameter.Descendants("weather-conditions")
                                     where conditionElements.Attribute("weather-summary").Value != ""
                                     select conditionElements;
                    foreach (var condition in conditions)
                    {
                        pointWeatherConditions.Add(condition.Attribute("weather-summary").Value);
                    }

                    // Loop through daily weather records for the current point and update it with the weather data
                    // gathered above.
                    // Loop through in the order the records were added, which will also be by date from current to future
                    // (same order as the temps and forecast info)
                    // ---------------------------------------------------------------------------------------
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "SELECT * FROM daily_weather WHERE ";
                    sqlQuery += "dw_point_key = '" + filePointKeys[i] + "' ORDER BY dw_key ASC";
                    dbcmd.CommandText = sqlQuery;
                    dbreader = dbcmd.ExecuteReader();
                    var m = 0;
                    while (dbreader.Read())
                    {
                        dbUpdatecmd = dbconn.CreateCommand();
                        sqlUpdateQuery = "UPDATE [daily_weather] SET ";
                        sqlUpdateQuery += "[dw_max_temp] = '" + pointMaxTemps[m] + "', ";
                        sqlUpdateQuery += "[dw_min_temp] = '" + pointMinTemps[m] + "', ";
                        sqlUpdateQuery += "[dw_conditions] = '" + pointWeatherConditions[m] + "' ";
                        sqlUpdateQuery += "WHERE [dw_key] = " + dbreader["dw_key"];
                        dbUpdatecmd.CommandText = sqlUpdateQuery;
                        dbUpdatecmd.ExecuteScalar();
                        m++;
                    }
                } // thisParameter
            } // filePointKeys.Count

            dbconn.Close();
            parserStatus = "Parsing Completed";
        } // dbconn
    } // start
}