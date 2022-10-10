// -------------------------------------------------------------------------------
//
// databaseManager.cs
// Description:   Define the app's database, initialize it and handle updates to
//                the database structure.
// Input:         (1) The required database version (set in Inspector window)
// Output:        (1) A control table
//                (2) Tables to store weather forecast data
// Created:       07 / 20 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;


public class databaseManager : MonoBehaviour
{
    string sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader dbreader;

    public float requiredDBVersion; // set in inspector window
    private string primedControlTable;
    private float currentDBVersion;

    // -------------------------------------------------------------------------------------------
    // Initialize the database tables
    // -------------------------------------------------------------------------------------------

    public void initDBTables()
    {
        primedControlTable = "NO";

        Debug.Log("filepath " + SceneParametersHandler.DATABASE_FILEPATH + " conn: " + SceneParametersHandler.DATABASE_CONN);
        

        // Create the database file if it doesn't already exist
        // -------------------------------------------------------------------------------------------
        if (!System.IO.File.Exists(SceneParametersHandler.DATABASE_FILEPATH))
        {
            Debug.Log("File DID NOT exist. Is being created.");
            File.Create(SceneParametersHandler.DATABASE_FILEPATH);
        }
        else
        {
            Debug.Log("File ALREADY EXISTS.");
        }


        using (dbconn = new SqliteConnection(SceneParametersHandler.DATABASE_CONN))
        {
            dbconn.Open();

            // Create the CONTROL table if it does not yet exist
            // -------------------------------------------------------------------------------------------
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "CREATE TABLE IF NOT EXISTS [control] (" +
                       "[control_key] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                       "[db_version] REAL DEFAULT NULL)";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            // Check to see if the CONTROL table is empty...
            // -------------------------------------------------------------------------------------------
            dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = "SELECT * FROM CONTROL LIMIT 1";
            dbreader = dbcmd.ExecuteReader();
            while (dbreader.Read())
            {
                primedControlTable = "YES";
                currentDBVersion = (float)dbreader["db_version"];
            }

            // The CONTROL table exists, but is empty, therefore, populate the table.
            // Happens only the first time the app is used. Set current db version to 0.
            // -------------------------------------------------------------------------------------------
            if (primedControlTable == "NO")
            {
                dbcmd = dbconn.CreateCommand();
                sqlQuery = "INSERT INTO [control] ( [control_key], [db_version] ) VALUES (1, 0)";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();
                currentDBVersion = 0f;
            }

            // Create the WEATHER_POINT table if it does not yet exist
            // -------------------------------------------------------------------------------------------
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "CREATE TABLE IF NOT EXISTS [weather_point] (" +
                       "[wp_key] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                       "[wp_point_key] TEXT DEFAULT NULL, " +
                       "[wp_latitude] REAL, " +
                       "[wp_longitude] REAL, " +
                       "[wp_last_updated] TEXT)";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            // Create the DAILY_WEATHER table if it does not yet exist
            // -------------------------------------------------------------------------------------------
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "CREATE TABLE IF NOT EXISTS [daily_weather] (" +
                       "[dw_key] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                       "[dw_point_key] TEXT DEFAULT NULL, " +
                       "[dw_datetime] TEXT, " +
                       "[dw_max_temp] INTEGER, " +
                       "[dw_min_temp] INTEGER, " +
                       "[dw_conditions] TEXT)";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbconn.Close();
         }

        // Does the database need to be updated?
        // -------------------------------------------------------------------------------------------
        dbUpdateCheck();
    }


    // -------------------------------------------------------------------------------------------
    // Initial population of the database (from version 0 to 1)
    // -------------------------------------------------------------------------------------------

    private void initialDBPopulation()
    {
        using (dbconn = new SqliteConnection(SceneParametersHandler.DATABASE_CONN))
        {
            dbconn.Open();

            // [ADD INITIAL DATA POPULATION CODE HERE]


            // Update the CONTROL table's DB Version
            // -------------------------------------------------------------------------------------------
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE [control] SET [db_version] = 1.0 WHERE [control_key] > 0";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
 
            dbconn.Close();
        }

        // Check for more required database updates
        // -------------------------------------------------------------------------------------------
        currentDBVersion = 1.0f;
        dbUpdateCheck();
    }


    // -------------------------------------------------------------------------------------------
    // Call database update routines based on the current version of the database
    // -------------------------------------------------------------------------------------------

    void dbUpdateCheck()
    {
        if (currentDBVersion == 0f)
        {
            // * Initial Population
            Debug.Log("Current Version = 0: initial population");
            initialDBPopulation();
        }
        else if (currentDBVersion == 1.0f && (requiredDBVersion > currentDBVersion))
        {
            // * Verson 1.0 Population
            Debug.Log("Required Version > 1.0 update from Current = 1.0");
            // updateV1toVxx();
        }
        // Future version updates here...
    }
}