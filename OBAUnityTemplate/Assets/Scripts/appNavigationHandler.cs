// -------------------------------------------------------------------------------
//
// appNavigationHandler.cs
// Description:   The routine to handle all the scene navigation button requests in one place.
//                If navigating to a scene from a table row then store data to be
//                retrieved by the next scene.
// Input:         (1) name of the scene to navigate to
//                (2) how to transition to the scene ("fade" only for now)
// Created:       07 / 13 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// -------------------------------------------------------------------------------
// Handle the app's navigation requests from the Player
// -------------------------------------------------------------------------------
public class appNavigationHandler : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public string toScene;
    public string screenTransition;


    // Transition to the requested scene
    // -------------------------------------------------------------------------------
	public void ChangeScene()
    {
        Debug.Log("Scene Name: " + toScene + " - " + screenTransition);
        SceneManager.LoadScene (toScene);
    }

    public void OnPointerClick(PointerEventData data)
    {
        Debug.Log("OnPointerClick called.");
    }
    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log("Pressed button! --- " + name);
    }
    public void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("OnPointerEnter called.");
    }
    public void OnPointerExit(PointerEventData data)
    {
        Debug.Log("OnPointerExit called.");
    }

    // Button was pressed and released
    // -------------------------------------------------------------------------------
    public void OnPointerUp(PointerEventData data)
    {
        // buttonLabel.color = new Color32(255, 255, 255, 255);
        Debug.Log("Released button! --- " + EventSystem.current.currentSelectedGameObject.name);

        // If clicking on a button in a table row (tableCell prefab)
        if (EventSystem.current.currentSelectedGameObject.name == "cellArrowButton")
        {
            GameObject thisCell = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
            string recKey = thisCell.transform.Find("num").gameObject.GetComponent<TextMeshProUGUI>().text;
            Debug.Log("Button's Cell --- " + thisCell);
            Debug.Log("Button's Record Key --- " + recKey);

            // Store data to be passed to the next scene
            SceneParametersHandler.detailRecordKey = recKey;
        }

        ChangeScene();
    }
}
