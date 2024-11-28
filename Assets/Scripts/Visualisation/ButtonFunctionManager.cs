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

/*
 * Mapping of functionPanels
 * 
 * 0    -   Selection Dropdown (showing Dataset)
 * 1    -   Dropdown menu (log, cluster, gene_count etc.)
 * 2    -   Dropdown spatial,t-sne,umap
 * 3    -   LassoBtn (ROI selection)
 * 4    -   TissueBtn (H&E stain)
 * 5    -   Export/Import Group
 * 6    -   Export/Import Group
 * 7    -   SymbolSize Slider
 * 8    -   Transparency Slider
 * 9    -   Colour Gradient
 * 10   -   Side By Side Btn
 * 11   -   DropDown Menu for Visium special reads
 * 12   -   reset Camera View Button
 * 13   -   SVG Button
 * 14   -   ObjectGroup Button (move, rotate, resize 3D object)
 * 15   -   Binary Search - Gene is on Slider
 * 16   -   CopySlider - Side by Side toggle which dataset active
 * 17   -   EnterVRBtn
 * 18   -   Min Treshold slider
 * 19   -   TopMenuBtn  
 * 20   -   SideMenuBtn
 * 21   -   ClusterBtn
 * 22   -   ScreenshotBtn
 * 23   -   FiguresBtn (FigureViewer)
 * 24   -   SymbolHolder (Change Symbol Sphere, Square)
 */


public class ButtonFunctionManager : MonoBehaviour
{
    DataTransfer df;
    public List<GameObject> functionPanels = new List<GameObject>();

    /// <summary>
    /// Disable buttons depending on the SRT technique used. 
    /// Depending if the feature is supported by the SRT method used.
    /// Note: This is done manually.
    /// </summary>
    /// <param name="df">DataTransfer instance.</param>
    public void SetFunction(DataTransfer df)
    {
        this.df = df;
        
        functionPanels[16].SetActive(false);

        if (df.c18)
        {
            //C18
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            ////functionPanels[10].SetActive(false);
            functionPanels[11].SetActive(false);

        }

        else if ((df.visium && !df.visiumMultiple) && !df.c18)
        {
            functionPanels[8].SetActive(false);

        }

        else if ((df.visium || df.visiumMultiple) && !df.c18)
        {
            // visium multiple slices
            //functionPanels[10].SetActive(false);
            functionPanels[8].SetActive(false);
        }

        else if (df.tomoseq)
        {
            functionPanels[0].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[18].SetActive(false);
           //TODO: not referenced
//            functionPanels[12].SetActive(false);
        }
        else if (df.stomics)
        {
            functionPanels[0].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false); 
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
        }
        else if (df.xenium)
        {
            functionPanels[0].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);

        }
        else if (df.merfish)
        {
            functionPanels[0].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
        }
        else if (df.other)
        {
            // custom option
            functionPanels[0].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[13].SetActive(false);
        }

        if (df.objectUsed)
        {
            functionPanels[8].SetActive(false);
            functionPanels[14].SetActive(false);
        }
    }

    public void ToggleCopySlider()
    {
        functionPanels[16].SetActive(!functionPanels[16].activeSelf);
    }

    public void Disable_Buttons_For_VR()
    {
        functionPanels[12].SetActive(false);
        functionPanels[17].SetActive(false);
        functionPanels[19].SetActive(false);
        functionPanels[20].SetActive(false);
    }

    /// <summary>
    /// Disable Button by identifier, see ButtonFunctionManager notes for identifiers
    /// </summary>
    /// <param name="id"></param>
    public void Disable_Btn_By_Identifier(int id)
    {
        functionPanels[id].SetActive(false);
    }

    /// <summary>
    /// Enable Button by identifier, see ButtonFunctionManager notes for identifiers
    /// </summary>
    /// <param name="id"></param>
    public void Enable_Btn_By_Identifier(int id)
    {
        functionPanels[id].SetActive(false);
    }

}
