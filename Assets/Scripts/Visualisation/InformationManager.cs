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

public class InformationManager : MonoBehaviour
{
    public GameObject infoPanel;
    public TMP_Text infotext;


    public void ShowInfoPanel(GameObject button)
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }

        string infoText = "";

        switch (button.name)
        {
            case "visium_Download_Info":
                infoText = "More information can be found here: https://scanpy-tutorials.readthedocs.io/en/latest/spatial/basic-analysis.html";
                break;
            case "visium_Pipeline_load_Info":
                infoText = "More information can be found here: https://scanpy-tutorials.readthedocs.io/en/latest/spatial/basic-analysis.html";
                break;            
            case "xenium_Info":
                infoText = "More information can be found here: https://squidpy.readthedocs.io/en/stable/external_tutorials/tutorial_xenium.html";
                break;            
            case "vizgen_Info":
                infoText = "More information can be found here: https://squidpy.readthedocs.io/en/stable/external_tutorials/tutorial_vizgen.html";
                break;            
            case "tomoseq_Info":
                infoText = "More information on the natue of the tomo data and how to reconstruct the gene files can be found here: https://doi.org/10.1016/j.cell.2014.09.038";
                break;
        }
        setInfoPanelActive(infoText);
    }

    private void setInfoPanelActive(string infoText)
    {
        infoPanel.SetActive(true);
        infotext.text = infoText;
    }


}
