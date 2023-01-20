/*
* Copyright (c) 2023 Life Science Informatics (university of Konstanz, Germany)
* author: Denis Bienroth
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the Software
* is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
* OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
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
    private bool active = false;


    public void enterTT(GameObject go)
    {
        active = true;
        wait();
        if (active)
        {
            tooltipBox.SetActive(true);
            tooltipBox.GetComponentInChildren<TMP_Text>().text = getDescription(go.name);
            tooltipBox.transform.position = new Vector2(Input.mousePosition.x + 20, Input.mousePosition.y + 20);
        }

    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(10);
    }

    public void exitTT()
    {
        tooltipBox.SetActive(false);
        active = false;
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
            case "LockedBtn":
                x = "Unlock slice rotation";
                break;            
            case "UnlockedBtn":
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
            case "C18HeartSlider":
                x = "Set the transparency of the heart object";
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
            case "LassoAddorRemoveToggle":
                x = "Toggle to add or remove spots from selection";
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
            case "SettingBtn":
                x = "Open settings menu";
                break;              
            case "UnselectLassoBtn":
                x = "Undo all selections";
                break;
            case "SVGBtn":
                x = "Show spatially variable genes list";
                break;
            case "ScreenshotBtn":
                x = "Export image of current view. CTRL + K";
                break;
            case "ObjectGroupBtn":
                x = "Show 3D object movement options";
                break;            
            case "Move-3dObjectBtn":
                x = "Move 3d object using F,T,G,H keys and Shift, CTRL+M";
                break;            
            case "RotateObjectBtn":
                x = "Rotate 3d object using F,T,G,H keys and Shift, CTRL+R";
                break;            
            case "ResizeObjectBtn":
                x = "Resize 3d object using F,T,G,H keys and Shift, CTRL+L";
                break;
        }
        return x;
    }

}