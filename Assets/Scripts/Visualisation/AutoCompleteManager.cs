using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoCompleteManager : MonoBehaviour
{
    public GameObject InputGameObject;
    public List<string> geneNames;
    public GameObject btnPrefab;
    public GameObject scrollView;
    public List<GameObject> tempBtns;
    public GameObject toggle;
    public bool visium = false;
    public bool tomoseq = false;
    public bool stomics = false;
    public bool xenium = false;
    public bool merfish = false;

    public void setGeneNameList(List<string> geneNames)
    {
       // this.geneNames = new HashSet<string>(geneNames);
        this.geneNames = geneNames;
    }

    private void Start()
    {
       visium = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().visium;
       tomoseq = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().tomoseq;
       stomics = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().stomics;
       xenium = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().xenium;
       merfish = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().merfish;
    }

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
            // toogle if genenames contain the input or if gene starts with input
            if (!toggle.GetComponent<Toggle>().isOn)
            {
                if (x.ToLower().Contains(InputGameObject.GetComponent<TMP_InputField>().text.ToLower()))
                {
                    if (i < 15)
                    {
                        //create Button for each entry
                        // addListener with function to it and transfer button as GO
                        GameObject btn = Instantiate(btnPrefab);
                        btn.transform.SetParent(scrollView.transform);
                        btn.transform.localPosition = new Vector3(0, 0, 0);
                        btn.GetComponentInChildren<TMP_Text>().fontSize = 16;
                        btn.GetComponentInChildren<TMP_Text>().text = x;
                        tempBtns.Add(btn);
                        btn.GetComponent<Button>().onClick.AddListener(delegate
                        {

                            selectGene(btn);
                        });
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (toggle.GetComponent<Toggle>().isOn)
            {
                if (x.ToLower().StartsWith(InputGameObject.GetComponent<TMP_InputField>().text.ToLower()))
                {
                    if (i < 10)
                    {
                        //create Button for each entry
                        // addListener with function to it and transfer button as GO
                        GameObject btn = Instantiate(btnPrefab);
                        btn.transform.SetParent(scrollView.transform);
                        btn.transform.localPosition = new Vector3(0, 0, 0);
                        btn.GetComponentInChildren<TMP_Text>().fontSize = 14;
                        btn.GetComponentInChildren<TMP_Text>().text = x;
                        tempBtns.Add(btn);
                        btn.GetComponent<Button>().onClick.AddListener(delegate
                        {

                            selectGene(btn);
                        });
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    // Pass gene to SearchManager to read expression values
    private void selectGene(GameObject btn)
    {

        InputGameObject.GetComponent<TMP_InputField>().text = btn.GetComponentInChildren<TMP_Text>().text;

        //TBD indexof genenames transfer to read hdf

            GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().resetNormalisedValues();

        if (GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().c18_visium) GameObject.Find("ScriptHolder").GetComponent<SearchManager>().readC18Expression(btn.GetComponentInChildren<TMP_Text>().text);
        else if (visium) GameObject.Find("ScriptHolder").GetComponent<SearchManager>().readExpressionList(btn.GetComponentInChildren<TMP_Text>().text);
        else if (tomoseq) GameObject.Find("ScriptHolder").GetComponent<TomoSeqDrawer>().runSearchTomo(btn.GetComponentInChildren<TMP_Text>().text);
        else if (stomics)
        {
            int pos = geneNames.IndexOf(btn.GetComponentInChildren<TMP_Text>().text);
            GameObject.Find("ScriptHolder").GetComponent<SearchManager>().readStomicsExpression(btn.GetComponentInChildren<TMP_Text>().text, pos);
            
        }
        else if (xenium) GameObject.Find("ScriptHolder").GetComponent<SearchManager>().readXeniumExpression(btn.GetComponentInChildren<TMP_Text>().text);
        else if (merfish) GameObject.Find("ScriptHolder").GetComponent<SearchManager>().readMerfishExpression(btn.GetComponentInChildren<TMP_Text>().text);
    }

    //check if Inoutfield is focused
    public bool InputFocused()
    {
        return InputGameObject.GetComponent<TMP_InputField>().isFocused;
    }
}
