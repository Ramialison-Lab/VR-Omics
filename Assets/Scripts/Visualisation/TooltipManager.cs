/*
* Copyright (c) 2023 Murdoch Children's Research Institute, Parkville, Melbourne
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
        string tooltip_text ="";
        switch (name)
        {
            case "SideBySideBtn": 
                tooltip_text = "Activate a duplicate of the current dataset";
                break;
            case "ExportBtn":
                tooltip_text = "Save the selected ROIs";
                break;
            case "LoadGroups":
                tooltip_text = "Load saved ROIs";
                break;
            case "HnEBtn":
                tooltip_text = "Show H&E overlay";
                break;
            case "LassoBtn":
                tooltip_text = "Show Selection tool options";
                break;                       
            case "LightBtn":
                tooltip_text = "Change background colour";
                break;            
            case "LockedBtn":
                tooltip_text = "Unlock slice rotation";
                break;            
            case "UnlockedBtn":
                tooltip_text = "Lock slice rotation";
                break;            
            case "SearchWithToggle":
                tooltip_text = "Search Genes containg the input";
                break;            
            case "SearchIF":
                tooltip_text = "Enter gene name";
                break;            
            case "CopySlider":
                tooltip_text = "Apply selection to the copy slice";
                break;             
            case "C18HeartSlider":
                tooltip_text = "Set the transparency of the heart object";
                break;            
            case "SelectionDropdown":
                tooltip_text = "Choose active dataset";
                break;            
            case "PassThroughBtn":
                tooltip_text = "Allows spot selection through slices";
                break;            
            case "0":
                tooltip_text = "Activate Group 1";
                break;                  
            case "1":
                tooltip_text = "Activate Group 2";
                break;                
            case "2":
                tooltip_text = "Activate Group 3";
                break;                
            case "3":
                tooltip_text = "Activate Group 4";
                break;                
            case "LassoAddorRemoveToggle":
                tooltip_text = "Toggle to add or remove spots from selection";
                break;                
            case "CSVBtn":
                tooltip_text = "Export selection as CSV file";
                break;                
            case "ImageBtn":
                tooltip_text = "Export an image of the current view";
                break;                
            case "ColorThresholdMin":
                tooltip_text = "Set minimal threshold for expression";
                break;                
            case "SphereSizeSlider":
                tooltip_text = "Set the size of the symbol";
                break;               
            case "SettingBtn":
                tooltip_text = "Open settings menu";
                break;              
            case "UnselectLassoBtn":
                tooltip_text = "Undo all selections";
                break;
            case "SVGBtn":
                tooltip_text = "Show spatially variable genes list";
                break;
            case "ScreenshotBtn":
                tooltip_text = "Create image of current view. CTRL + K";
                break;
            case "ObjectGroupBtn":
                tooltip_text = "Show 3D object movement options";
                break;            
            case "Move-3dObjectBtn":
                tooltip_text = "Move 3d object using F,T,G,H keys and Shift, CTRL+M";
                break;            
            case "RotateObjectBtn":
                tooltip_text = "Rotate 3d object using F,T,G,H keys and Shift, CTRL+R";
                break;            
            case "ResizeObjectBtn":
                tooltip_text = "Resize 3d object using F,T,G,H keys and Shift, CTRL+L";
                break;            
            case "ClusterBtn":
                tooltip_text = "Visualise cluster information";
                break;               
            case "FiguresBtn":
                tooltip_text = "Show output plots";
                break;                
            case "SphereBtn":
                tooltip_text = "Change location symbol to cubes";
                break;                        
            case "CubeBtn":
                tooltip_text = "Change location symbol to spheres";
                break;             
            case "resetCameraBtn":
                tooltip_text = "Reset view";
                break;               
            case "EnterVRBtn":
                tooltip_text = "Change to VR environment";
                break;            
        }
        return tooltip_text;
    }

}