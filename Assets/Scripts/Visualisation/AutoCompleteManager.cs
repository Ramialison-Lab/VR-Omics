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
        GameObject btn = Instantiate(btnPrefab);
        btn.transform.SetParent(scrollView.transform);
        btn.transform.localPosition = new Vector3(0, 0, 0);
        btn.GetComponentInChildren<TMP_Text>().fontSize = 14;
        btn.GetComponentInChildren<TMP_Text>().text = geneName;
        tempBtns.Add(btn);
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
