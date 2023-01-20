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
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    public GameObject containerPrefab;
    public GameObject colourPanel;
    public List<GameObject> containerList;

    private void Start()
    {
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));

    }
    public void AddContainer()
    {
        if(containerList.Count<8) containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
    }

    public void RemoveContainer()
    {
        Destroy(containerList.Last());
        containerList.Remove(containerList.Last());
    }
    public List<string> colourParam;
    public void submitSelection()
    {
        colourParam.Clear();
        foreach(GameObject go in containerList)
        {
           // TMP_InputField txt = go.GetComponentInChildren<TMP_InputField>();

            colourParam.Add("NaN");
            InputField[] rgbs = go.GetComponentsInChildren<InputField>();
            foreach (InputField rgb in rgbs)
            {
                colourParam.Add(rgb.text);
            }

            TMP_Dropdown colourDropdown = go.GetComponentInChildren<TMP_Dropdown>();
            colourParam.Add(colourDropdown.options[colourDropdown.value].text);
        }

        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setColourScheme(colourParam);

    }

    public void defaultColour()
    {
        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().defaultColour();
    }
}
