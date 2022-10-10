// -------------------------------------------------------------------------------
//
// startMeUp.cs
// Description:   Script added to the first scene in the app - in this case "homeScene".
//                Handle any initial setup or initializations like creating the app's database
//                and making any required updates to the app's database.
// Created:       08 / 15 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using UnityEngine;
using TMPro;


public class startMeUp : MonoBehaviour
{
    public GameObject dbManager;
    public TextMeshProUGUI dbResults;

    // When the app is first launched
    void Awake()
    {
        // Initialize the app's database and make sure it is up-to-date
        // -------------------------------------------------------------------------------
        Debug.Log("* Start Me Up");
        var dbManagerScript = dbManager.GetComponent<databaseManager>();
        dbManagerScript.initDBTables();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Report database status
        if (!System.IO.File.Exists(SceneParametersHandler.DATABASE_FILEPATH))
            {
            Debug.Log("Database File DOES NOT exist.");
            dbResults.SetText("Database File DOES NOT exist.");
        }
        else
        {
            Debug.Log("Database File EXISTS.");
            dbResults.SetText("Database File EXISTS.");
        }
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
