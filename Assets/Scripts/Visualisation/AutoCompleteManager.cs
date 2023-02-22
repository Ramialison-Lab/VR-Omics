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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoCompleteManager : MonoBehaviour
{
    public List<string> geneNames;

    //Gameobjects
    public GameObject InputGameObject;
    public GameObject btnPrefab;
    public GameObject scrollView;
    public GameObject InputContainsToggle;
    public List<GameObject> tempBtns;

    // Access variables
    private DataTransferManager dfm;
    private SearchManager sm;
    private TomoSeqDrawer tsd;
    private SpotDrawer sd;
    private TMP_InputField tmp_input;
    public GameObject viewPortGo;


    private void Start()
    {
       dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
       sm = GameObject.Find("ScriptHolder").GetComponent<SearchManager>();
       tsd = GameObject.Find("ScriptHolder").GetComponent<TomoSeqDrawer>();
       sd = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
       tmp_input = InputGameObject.GetComponent<TMP_InputField>();
    }

    /// <summary>
    /// Set geneName list 
    /// </summary>
    /// <param name="geneNames">List of gene names in the current dataset</param>
    public void setGeneNameList(List<string> geneNames)
    {
        this.geneNames = geneNames;
    }

    /// <summary>
    /// On Value change method, reads input and compares to genelist, find ind results matching the input. Creates a list of buttons with matching values.
    /// </summary>
    public void textEnter()
    {
        int i = 0;
        foreach (GameObject x in GameObject.FindGameObjectsWithTag("contentBtn"))
        {
            Destroy(x);
        }

        if (InputGameObject.GetComponent<TMP_InputField>().text == "") { return; }
        foreach (string x in geneNames)
        {
            if (i > 100) break;
            // toogle if genenames contain the input or if gene starts with input
            if (!InputContainsToggle.GetComponent<Toggle>().isOn)
            {
                if (x.ToLower().Contains(tmp_input.text.ToLower()))
                {
                    {
                        populateResultBtns(x);
                        i++;
                        viewPortGo.GetComponent<RectTransform>().sizeDelta = new Vector2(viewPortGo.GetComponent<RectTransform>().rect.width, (i * 30f));

                    }
                }
            }
            else if (InputContainsToggle.GetComponent<Toggle>().isOn)
            {
                if (x.ToLower().StartsWith(InputGameObject.GetComponent<TMP_InputField>().text.ToLower()))
                {
                    {
                        populateResultBtns(x);
                        i++;
                        viewPortGo.GetComponent<RectTransform>().sizeDelta = new Vector2(viewPortGo.GetComponent<RectTransform>().rect.width, (i * 30f));

                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds resultbutton with description geneName
    /// </summary>
    /// <param name="x">the name of the gene to be shown as button description</param>
    private void populateResultBtns(string geneName)
    {
        //GameObject btn = Instantiate(btnPrefab, viewPortGo.transform);
        GameObject btn = Instantiate(btnPrefab);
        //btn.transform.rotation = Quaternion.Euler(0, 0, 0);

        btn.transform.SetParent(scrollView.transform);
        btn.transform.localPosition = new Vector3(0, 0, 0);
        if (dfm.svgGenes.Contains(geneName)) { btn.GetComponent<Image>().color = Color.cyan; }
        btn.GetComponentInChildren<TMP_Text>().fontSize = 14;
        btn.GetComponentInChildren<TMP_Text>().text = geneName;
        tempBtns.Add(btn);
        if (sd.inVR)
        {
            btn.transform.localScale = new Vector3(1, 1, 1);
        }
        btn.GetComponent<Button>().onClick.AddListener(delegate
        {
            selectGene(btn);
        });
    }


    /// <summary>
    /// Passes the selected gene to the respective method to read out the corrseponding expression value matrix
    /// </summary>
    /// <param name="btn"></param>
    public void selectGene(GameObject btn)
    {
        TMP_Text tmp_txt = btn.GetComponentInChildren<TMP_Text>();
        InputGameObject.GetComponent<TMP_InputField>().text = tmp_txt.text;

        //TBD indexof genenames transfer to read hdf

        sd.resetNormalisedValues();
        GetComponent<ReadClusterInformation>().resetClusterInfoPanel();
        if (dfm.c18_visium) sm.readC18Expression(tmp_txt.text);
        else if (dfm.visium) sm.readExpressionList(tmp_txt.text);
        else if (dfm.tomoseq) tsd.runSearchTomo(tmp_txt.text);
        else if (dfm.stomics)
        {
            int pos = geneNames.IndexOf(tmp_txt.text);
            sm.readStomicsExpression(tmp_txt.text, pos);   
        }
        else if (dfm.xenium) sm.readXeniumExpression(tmp_txt.text);
        else if (dfm.merfish) sm.readMerfishExpression(tmp_txt.text);
    }

    /// <summary>
    /// Check if Inputfield for gene input is focused to avoid movement to be entered into search bar
    /// </summary>
    /// <returns></returns>
    public bool InputFocused()
    {
        return InputGameObject.GetComponent<TMP_InputField>().isFocused;
    }
}
