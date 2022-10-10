// -------------------------------------------------------------------------------
//
// loadMap.cs
// Description:   Load maps into the UniWebView object.
// Created:       08 / 18 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class loadMap : MonoBehaviour
{
    // Load the Terrain Map
    public void loadTerrainMap()
    {
        UniWebView webView = FindObjectOfType<UniWebView>();
        webView.Load("https://www.google.com/maps/@42.4427334,-76.5158845,14z");
    }

    // Load the Topo Map
    public void loadTopoMap()
    {
        UniWebView webView = FindObjectOfType<UniWebView>();
        webView.Load("https://mappingsupport.com/p2/gissurfer.php?center=44.143645,-73.986866&zoom=16&basemap=USA_basemap");
    }
}