// -------------------------------------------------------------------------------
//
// getInTouchHandler.cs
// Description:   Ways the app user can get in touch for help and more info.
// Created:       07 / 20 / 22 © One Bad Ant
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

public class getInTouchHandler : MonoBehaviour, IPointerUpHandler
{
    public TextMeshProUGUI buttonLabel;
    public Button navButton;
    public string contactHow;

    // Get in touch via email 
    void contactViaEmail()
    {
        string t = "mailto:ant@onebadant.com?subject=Unity%20Template%20Test&body=";
        Application.OpenURL(t);
    }

    // Get in touch via website 
    void contactViaWebsite()
    {
        string t = "https://www.onebadant.com";
        Application.OpenURL(t);
    }

    // Button was pressed and released
    // -------------------------------------------------------------------------------
    public void OnPointerUp(PointerEventData data)
    {
        // buttonLabel.color = new Color32(255, 255, 255, 255);
        Debug.Log("Released button! --- " + name);

        if (contactHow == "Email") { contactViaEmail(); }
        if (contactHow == "Website") { contactViaWebsite(); }
    }
}
