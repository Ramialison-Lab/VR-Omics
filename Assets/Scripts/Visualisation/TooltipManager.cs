using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipManager : MonoBehaviour
{

    public string tooltipText = "";
    public GameObject tooltipBox;


    public void enterTT(GameObject go)
    {
        tooltipBox.SetActive(true);
        tooltipBox.GetComponentInChildren<TMP_Text>().text = getDescription(go.name);
        tooltipBox.transform.position = new Vector2(Input.mousePosition.x +20, Input.mousePosition.y+20);

    }

    public void exitTT()
    {
        tooltipBox.SetActive(false);
    }

    private string getDescription(string name)
    {
        string x ="";
        switch (name)
        {
            case "SideBySideBtn": x = "Activate a second slice to compare expression patterns";
                break;
            case "ExportBtn":
                x = "Show Export Options";
                break;
            case "HnEBtn":
                x = "Show H&E overlay";
                break;
            case "LassoBtn":
                x = "Show Selection tool options";
                break;                       
            case "LightBtn":
                x = "Disable background for better contrast";
                break;            
            case "UnlockBtn":
                x = "Unlock slice rotation";
                break;            
            case "LockBtn":
                x = "Lock slice rotation";
                break;            
            case "SearchWithToggle":
                x = "Search Genes containg the input";
                break;            
            case "SearchIF":
                x = "Enter gene name";
                break;            
            case "CopySlider":
                x = "Apply selection to the copy slice";
                break;            
            case "SelectionDropdown":
                x = "Choose active dataset";
                break;            
            case "PassThroughBtn":
                x = "Allows spot selection through slices";
                break;            
            case "0":
                x = "Activate Group 1";
                break;                  
            case "1":
                x = "Activate Group 2";
                break;                
            case "2":
                x = "Activate Group 3";
                break;                
            case "3":
                x = "Activate Group 4";
                break;                
            case "LassoDuplicateCopySlider":
                x = "Transfer selection to copy slide";
                break;                
            case "CSVBtn":
                x = "Export selection as CSV file";
                break;                
            case "ImageBtn":
                x = "Export an image of the current view";
                break;                
            case "ColorThresholdMin":
                x = "Set minimal threshold for expression";
                break;                
            case "SphereSizeSlider":
                x = "Set the size of the symbol";
                break;               
            case "SettingsBtn":
                x = "Open settings menu";
                break;                

        }


        return x;
    }

}