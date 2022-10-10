// -------------------------------------------------------------------------------
//
// scrollScript.cs
// Description:   Create a simple table of rows with cells including a number
//                and button leading to a data detail screen.
//                Each cell is added to a content area inside a scrollable area.
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

public class scrollScript : MonoBehaviour
{

    public ScrollRect scrollView;
    public GameObject scrollContent;
    public GameObject scrollItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for ( int a = 1; a<= 20; a++ )
        {
            generateCell(a);
        }
        scrollView.verticalNormalizedPosition = 1;
    }

    void generateCell(int itemNumber)
    {
        GameObject scrollItemObj = Instantiate(scrollItemPrefab);
        scrollItemObj.transform.SetParent(scrollContent.transform, false);
        scrollItemObj.transform.Find("num").gameObject.GetComponent<TextMeshProUGUI>().text = itemNumber.ToString();
    }
}
