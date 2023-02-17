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

public class ButtonFunctionManager : MonoBehaviour
{
    DataTransfer df;
    public List<GameObject> functionPanels = new List<GameObject>();

    public void setFunction(DataTransfer df)
    {
        this.df = df;
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
            functionPanels[16].SetActive(false);

        }
        else if ((df.visium && !df.visiumMultiple) && !df.c18)
        {
            functionPanels[8].SetActive(false);
            functionPanels[16].SetActive(false);
            functionPanels[17].SetActive(false);

        }

        else if ((df.visium || df.visiumMultiple) && !df.c18)
        {
            // visium multiple slices
            //functionPanels[10].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[16].SetActive(false);
            functionPanels[17].SetActive(false);


        }
        else if (df.tomoseq)
        {
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
           //TODO: not referenced
//            functionPanels[12].SetActive(false);
            functionPanels[16].SetActive(false);
        }
        else if (df.stomics)
        {
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false); 
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[16].SetActive(false);
        }
        else if (df.xenium)
        {
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[16].SetActive(false);

        }
        else if (df.merfish)
        {
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[16].SetActive(false);
        }
        else if (df.other)
        {
            // custom option
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            functionPanels[8].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[13].SetActive(false);
            functionPanels[16].SetActive(false);


        }

        if (df.objectUsed)
        {
            functionPanels[8].SetActive(false);
            functionPanels[14].SetActive(false);
        }


    }

    public void ToggleCopySlider()
    {
        if (functionPanels[16].activeSelf)
        {
            functionPanels[16].SetActive(false);
        }
        else
        {
            functionPanels[16].SetActive(true);
        }

    }
}
