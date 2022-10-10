// -------------------------------------------------------------------------------
//
// SceneParametersHandler.cs
// Description:   Define and hold data to be passed from one scene to another.
//                Data is set in this routine by the "appNavigationHandler".
//                Individual scenes needing passed data will find it here.
//                This is a static class that does not need to be attached to any game object.
// Created:       07 / 25 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SceneParametersHandler
{
    // Define Database Settings
    // Determine the database's file path for Unity editor or other (iOS/Android)
    // Define path in which to store photos selected and added to the app
    // -------------------------------------------------------------------------------------------
    public const string DATABASE_NAME = "obappdb.db";

#if UNITY_EDITOR
    public static string DATABASE_FILEPATH = string.Format(@"Assets/StreamingAssets/{0}", DATABASE_NAME);
    public static string DATABASE_CONN = string.Format(@"URI=file:Assets/StreamingAssets/{0}", DATABASE_NAME);
    public static string PHOTOS_PATH = "Assets/Photos/";
#else
    public static string DATABASE_FILEPATH = string.Format("{0}/{1}", Application.persistentDataPath, DATABASE_NAME);
    public static string DATABASE_CONN = string.Format(@"URI=file:{0}", DATABASE_FILEPATH);
    public static string PHOTOS_PATH = Application.persistentDataPath;
#endif


    // Hold Data to be passed from one scene to another
    // -------------------------------------------------------------------------------------------
    public static string detailRecordKey { get; set; }  // Key from record in row of listScene
}