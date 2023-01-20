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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisiumParameters : MonoBehaviour
{
    public Dropdown m_Dropdown;
    public string str = "";
    public GameObject DownloadUI;
    public GameObject p_selection;
    public List<string> paramList = new List<string>();
    public GameObject warningMessage;
    public GameObject w_text_format;

    // Called if dropdown value is changed → selected database parameter
    public void dropdownValueChanged()
    {
        str = GameObject.Find("m_dropdown").GetComponent<Dropdown>()
            .options[GameObject.Find("m_dropdown").GetComponent<Dropdown>().value].text;
    }

    // called if database was selected to enter the filter parameters
    public void startParameterSelection()
    {

        if (str == "")
        {
            warningMessage.SetActive(true);
        }
        else
        {
            DownloadUI.SetActive(false);
            p_selection.SetActive(true);
            if (warningMessage.activeSelf) warningMessage.SetActive(false);
        }
    }

    // called to get all filter parameters from the input fields
    public void getAllParametersFromInputFields()
    {
        int number;
        foreach (InputField inputField in p_selection.GetComponentsInChildren<InputField>())
        {
            foreach (Text text in inputField.GetComponentsInChildren<Text>())
            {
                if (int.TryParse(text.text, out number))
                {
                    paramList.Add(text.text);
                }
                else
                {
                    w_text_format.SetActive(true);
                    paramList.Clear();
                    break;
                }
            }
        }

        //TBD
        // these parameters need to be passed to the notebook
        // str is the paramter for the sampl_id in scanpy https://scanpy.readthedocs.io/en/stable/generated/scanpy.datasets.visium_sge.html
        // paramList holds all parameters for the filter section of the notebook 

    }

}
