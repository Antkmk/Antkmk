// -------------------------------------------------------------------------------
//
// displayPassedParameters.cs
// Description:   Routine attached to an empty game object that is part of a scene
//                needing to display data passed to it from another scene via
//                "SceneParametersHandler".
// Created:       08 / 16 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class displayPassedParameters : MonoBehaviour
{
    public TextMeshProUGUI rowNumber;

    void Start()
    {
        rowNumber.SetText(SceneParametersHandler.detailRecordKey);
    }
}
